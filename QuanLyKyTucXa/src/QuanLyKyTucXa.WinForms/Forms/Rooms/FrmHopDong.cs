using System.Globalization;
using QuanLyKyTucXa.Infrastructure.Security;
using QuanLyKyTucXa.Infrastructure.Services;

namespace QuanLyKyTucXa.WinForms.Forms.Rooms;

public sealed class FrmHopDong : Form
{
    private readonly string _connectionString; private readonly AuthSession? _session;
    private readonly DataGridView _grid=new(){Dock=DockStyle.Fill,ReadOnly=true,AutoGenerateColumns=true,AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill,SelectionMode=DataGridViewSelectionMode.FullRowSelect,MultiSelect=false};
    private readonly TextBox _search=new(){Width=220,PlaceholderText="Mã HĐ, sinh viên, phòng"};
    public event EventHandler? ReturnHomeRequested;
    private RoomOperationsService Service=>new(()=>QuanLyKyTucXa.Infrastructure.Data.Context.AppDbContextFactory.Create(_connectionString),_session);
    public FrmHopDong(string connectionString,AuthSession? session){_connectionString=connectionString;_session=session;Text="Hợp đồng lưu trú";Width=1100;Height=650;StartPosition=FormStartPosition.CenterParent;BuildUi();Load+=async(_,_)=>await ReloadAsync();}
    public Task RefreshViewAsync() => ReloadAsync();
    private void BuildUi()
    {
        _grid.DataBindingComplete += (_,_) => SetGridHeaders();
        _grid.CellFormatting += (_, e) => FormatFinancialCells(e);
        var bar=new FlowLayoutPanel{Dock=DockStyle.Top,Height=48,Padding=new Padding(8)};
        var find=new Button{Text="Tìm",AutoSize=true};find.Click+=async(_,_)=>await ReloadAsync();_search.KeyDown+=async(_,e)=>{if(e.KeyCode==Keys.Enter){e.SuppressKeyPress=true;await ReloadAsync();}};
        var transfer=new Button{Text="Chuyển phòng",AutoSize=true};transfer.Click+=async(_,_)=>await TransferAsync();
        var renew=new Button{Text="Gia hạn",AutoSize=true};renew.Click+=async(_,_)=>await RenewAsync();
        var checkout=new Button{Text="Trả phòng",AutoSize=true};checkout.Click+=async(_,_)=>await CheckoutAsync();
        var history=new Button{Text="Lịch sử / bàn giao",AutoSize=true};history.Click+=async(_,_)=>await ShowHistoryAsync();
        var refresh=new Button{Text="Làm mới",AutoSize=true};refresh.Click+=async(_,_)=>await ReloadAsync();
        var close=new Button{Text="Đóng",AutoSize=true};close.Click+=(_,_)=>ReturnHomeRequested?.Invoke(this,EventArgs.Empty);
        transfer.Visible=renew.Visible=checkout.Visible=AuthPolicy.Can(_session,AppPermission.Staff);bar.Controls.AddRange([_search,find,transfer,renew,checkout,history,refresh,close]);Controls.Add(_grid);Controls.Add(bar);
    }
    private void SetGridHeaders()
    {
        SetHeader("ContractId","Mã hợp đồng");SetHeader("StudentId","Mã sinh viên");SetHeader("StudentName","Sinh viên");
        SetHeader("RoomId","Mã phòng");SetHeader("BedId","Mã giường");SetHeader("Start","Bắt đầu");SetHeader("End","Kết thúc");
        SetHeader("RoomPrice","Tiền/tháng (đ)");SetHeader("Status","Trạng thái");SetHeader("Outstanding","Công nợ (đ)");
        if(_grid.Columns["History"] is{} history)history.Visible=false;
        if(_grid.Columns["Start"] is{} start)start.DefaultCellStyle.Format="dd/MM/yyyy";
        if(_grid.Columns["End"] is{} end)end.DefaultCellStyle.Format="dd/MM/yyyy";
        if(_grid.Columns["RoomPrice"] is{} price)price.DefaultCellStyle.Format="N0";
        if(_grid.Columns["Outstanding"] is{} outstanding)outstanding.DefaultCellStyle.Format="N0";
        SetColumnWeight("ContractId",115);SetColumnWeight("StudentId",95);SetColumnWeight("StudentName",145);SetColumnWeight("RoomId",80);
        SetColumnWeight("BedId",105);SetColumnWeight("Start",85);SetColumnWeight("End",85);SetColumnWeight("RoomPrice",110);
        SetColumnWeight("Status",90);SetColumnWeight("Outstanding",110);
    }
    private void SetHeader(string property,string title){if(_grid.Columns[property] is{} column)column.HeaderText=title;}
    private void SetColumnWeight(string property,float weight){if(_grid.Columns[property] is{} column)column.FillWeight=weight;}
    private void FormatFinancialCells(DataGridViewCellFormattingEventArgs e)
    {
        if(e.RowIndex<0||_grid.Columns[e.ColumnIndex].Name!="Outstanding"||e.Value is not decimal amount||amount<=0)return;
        e.CellStyle.BackColor=Color.FromArgb(255,232,232);e.CellStyle.ForeColor=Color.FromArgb(164,40,40);
        e.CellStyle.SelectionBackColor=e.CellStyle.BackColor;e.CellStyle.SelectionForeColor=e.CellStyle.ForeColor;
    }
    private ContractRoomRow? Selected()=>_grid.CurrentRow?.DataBoundItem as ContractRoomRow;
    private async Task ReloadAsync(){try{_grid.DataSource=await Service.GetContractsAsync(_search.Text);}catch(Exception ex){Error(ex);}}
    private async Task TransferAsync()
    {
        var row=Selected();if(row is null)return;
        try
        {
            var beds=await Service.GetAvailableBedsAsync(row.ContractId);if(beds.Count==0){MessageBox.Show(this,"Không có giường trống phù hợp.","Chuyển phòng");return;}
            using var dialog=new BedPickerDialog(beds);if(dialog.ShowDialog(this)!=DialogResult.OK||dialog.SelectedBed is null)return;
            var checklist=await AssetService.GetChecklistAsync(row.ContractId);
            using var handover=new HandoverDialog(checklist,"Xác nhận chuyển phòng");if(handover.ShowDialog(this)!=DialogResult.OK)return;
            await Service.TransferAsync(row.ContractId,dialog.SelectedBed.BedId,handover.Note,handover.AssetsInspected,
                handover.CompensationCollected,handover.PaymentMethod,handover.Items);
            await ReloadAsync();
            MessageBox.Show(this,"Đã lưu biên bản bàn giao phòng cũ, nhận phòng mới và cập nhật hợp đồng trong cùng transaction.","Chuyển phòng",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }
        catch(Exception ex){Error(ex);}
    }
    private async Task RenewAsync()
    {
        var row=Selected();if(row is null)return;
        if(row.Outstanding>0){MessageBox.Show(this,$"Hợp đồng còn công nợ {row.Outstanding:N0} đồng. Cần thanh toán trước khi gia hạn.","Chưa thể gia hạn",MessageBoxButtons.OK,MessageBoxIcon.Warning);return;}
        using var dialog=new RenewalDateDialog(row.End);if(dialog.ShowDialog(this)!=DialogResult.OK)return;
        try{await Service.RenewAsync(row.ContractId,dialog.NewEndDate);await ReloadAsync();MessageBox.Show(this,"Đã gia hạn hợp đồng.","Gia hạn",MessageBoxButtons.OK,MessageBoxIcon.Information);}
        catch(Exception ex){Error(ex);}
    }
    private async Task CheckoutAsync()
    {
        var row=Selected();if(row is null)return;
        if(row.Outstanding>0){MessageBox.Show(this,$"Hợp đồng còn công nợ {row.Outstanding:N0} đồng. Cần thu đủ trước khi trả phòng.","Chưa thể trả phòng",MessageBoxButtons.OK,MessageBoxIcon.Warning);return;}
        try
        {
            var checklist=await AssetService.GetChecklistAsync(row.ContractId);
            using var handover=new HandoverDialog(checklist,"Xác nhận trả phòng");if(handover.ShowDialog(this)!=DialogResult.OK)return;
            await Service.CheckoutAsync(row.ContractId,handover.Note,handover.AssetsInspected,
                handover.CompensationCollected,handover.PaymentMethod,handover.Items);
            await ReloadAsync();MessageBox.Show(this,"Đã lưu biên bản bàn giao, thanh lý hợp đồng và cập nhật giường trống.","Trả phòng",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }
        catch(Exception ex){Error(ex);}
    }
    private RoomAssetHandoverService AssetService=>new(()=>QuanLyKyTucXa.Infrastructure.Data.Context.AppDbContextFactory.Create(_connectionString),_session);
    private async Task ShowHistoryAsync()
    {
        var row=Selected();if(row is null)return;
        try
        {
            var minutes=await AssetService.GetHistoryAsync(row.ContractId);
            var lines=new List<string>{$"Hợp đồng: {row.ContractId}",$"Sinh viên: {row.StudentName} ({row.StudentId})",$"Phòng/giường hiện tại: {row.RoomId} / {row.BedId}","",
                "LỊCH SỬ HỢP ĐỒNG",string.IsNullOrWhiteSpace(row.History)?"Chưa có ghi chú.":row.History!,"","BIÊN BẢN BÀN GIAO"};
            if(minutes.Count==0)lines.Add("Chưa có biên bản chi tiết.");
            foreach(var minute in minutes.GroupBy(x=>x.MinuteId).Select(x=>x.First()))
            {
                lines.Add($"{minute.Type} — {minute.Date:dd/MM/yyyy HH:mm} — phòng {minute.RoomId}, giường {minute.BedId} — người lập: {minute.Recorder}");
                if(!string.IsNullOrWhiteSpace(minute.MinuteNote))lines.Add($"  Ghi chú: {minute.MinuteNote}");
                foreach(var item in minutes.Where(x=>x.MinuteId==minute.MinuteId&&x.AssetName is not null))
                    lines.Add($"  • {item.AssetName}: {item.QuantityGiven} ({item.ConditionGiven}) → {item.QuantityNow} ({item.ConditionNow}); bồi thường {item.Compensation:N0} đ{(string.IsNullOrWhiteSpace(item.ItemNote)?"":$"; {item.ItemNote}")}");
            }
            using var dialog=new HandoverHistoryDialog(string.Join(Environment.NewLine,lines));dialog.ShowDialog(this);
        }
        catch(Exception ex){Error(ex);}
    }
    private void Error(Exception ex)=>MessageBox.Show(this,ex.Message,"Hợp đồng",MessageBoxButtons.OK,MessageBoxIcon.Warning);
}

internal sealed class RenewalDateDialog:Form
{
    private readonly DateTimePicker _end=new(){Format=DateTimePickerFormat.Custom,CustomFormat="dd/MM/yyyy",Dock=DockStyle.Top};
    public DateOnly NewEndDate=>DateOnly.FromDateTime(_end.Value);
    public RenewalDateDialog(DateOnly currentEnd)
    {
        Text="Gia hạn hợp đồng";Width=400;Height=170;StartPosition=FormStartPosition.CenterParent;
        var min=DateTime.Today.AddDays(1);var current=currentEnd.ToDateTime(TimeOnly.MinValue);if(current>=min)min=current.AddDays(1);
        _end.MinDate=min;_end.Value=min;
        var label=new Label{Text=$"Ngày kết thúc hiện tại: {currentEnd:dd/MM/yyyy}. Chọn ngày kết thúc mới:",Dock=DockStyle.Top,Height=36,TextAlign=ContentAlignment.MiddleLeft};
        var save=new Button{Text="Gia hạn",DialogResult=DialogResult.OK};var cancel=new Button{Text="Hủy",DialogResult=DialogResult.Cancel};
        var buttons=new FlowLayoutPanel{Dock=DockStyle.Bottom,Height=40,FlowDirection=FlowDirection.RightToLeft};buttons.Controls.AddRange([cancel,save]);
        Controls.Add(_end);Controls.Add(label);Controls.Add(buttons);AcceptButton=save;CancelButton=cancel;
    }
}

internal sealed class BedPickerDialog:Form
{
    private readonly ComboBox _beds=new(){Dock=DockStyle.Top,DropDownStyle=ComboBoxStyle.DropDownList,FormattingEnabled=true};
    public AvailableBedRow? SelectedBed=>_beds.SelectedItem as AvailableBedRow;
    public BedPickerDialog(List<AvailableBedRow> beds){Text="Chọn giường mới";Width=540;Height=170;StartPosition=FormStartPosition.CenterParent;_beds.Format+=(_,e)=>{if(e.ListItem is AvailableBedRow bed)e.Value=$"{bed.RoomId} / {bed.BedId} • {bed.TowerName} • {bed.RoomPrice:N0} đ/tháng";};foreach(var bed in beds)_beds.Items.Add(bed);if(_beds.Items.Count>0)_beds.SelectedIndex=0;var save=new Button{Text="Chuyển",DialogResult=DialogResult.OK};var cancel=new Button{Text="Hủy",DialogResult=DialogResult.Cancel};var bar=new FlowLayoutPanel{Dock=DockStyle.Bottom,FlowDirection=FlowDirection.RightToLeft,Height=42};bar.Controls.AddRange([cancel,save]);Controls.Add(_beds);Controls.Add(bar);AcceptButton=save;CancelButton=cancel;}
}

internal sealed class HandoverDialog:Form
{
    private readonly DataGridView _assets=new(){Dock=DockStyle.Fill,AllowUserToAddRows=false,AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill,SelectionMode=DataGridViewSelectionMode.CellSelect,MultiSelect=false};
    private readonly Label _baselineNotice=new(){Dock=DockStyle.Fill,TextAlign=ContentAlignment.MiddleLeft};
    private readonly TextBox _note=new(){MaxLength=180,PlaceholderText="Ghi chú bàn giao hoặc mô tả hư hỏng/mất mát",Width=340};
    private readonly CheckBox _inspected=new(){Text="Đã kiểm tra tài sản và thiết bị phòng",AutoSize=true};
    private readonly CheckBox _collected=new(){Text="Đã lập biên bản và thực thu khoản bồi thường",AutoSize=true};
    private readonly ComboBox _method=new(){DropDownStyle=ComboBoxStyle.DropDownList};
    private readonly Label _compensationTotal=new(){AutoSize=true,Padding=new Padding(0,7,8,0)};
    public string? Note=>string.IsNullOrWhiteSpace(_note.Text)?null:_note.Text.Trim();
    public bool AssetsInspected=>_inspected.Checked;
    public bool CompensationCollected=>_collected.Checked;
    public string PaymentMethod=>_method.Text;
    public IReadOnlyList<HandoverAssetLine> Items { get; private set; } = [];

    public HandoverDialog(HandoverChecklist checklist,string title)
    {
        Text=title;Width=1050;Height=650;StartPosition=FormStartPosition.CenterParent;
        _method.Items.AddRange(["Tiền mặt","Chuyển khoản VietQR","Chuyển khoản ngân hàng"]);_method.SelectedIndex=0;
        _baselineNotice.Text=checklist.HasSavedBaseline
            ? $"Biên bản nhận phòng gốc: {checklist.BaselineDate:dd/MM/yyyy}. Kiểm tra số lượng và tình trạng thực tế từng món."
            : "Hợp đồng cũ chưa có biên bản nhận phòng. Số liệu danh mục hiện tại được nạp làm mốc; hãy đối chiếu hồ sơ giấy trước khi xác nhận.";
        _baselineNotice.ForeColor=checklist.HasSavedBaseline?Color.DarkGreen:Color.DarkOrange;
        BuildAssetGrid(checklist.Items,checklist.HasSavedBaseline);
        var prompt=new Label{Text=$"Đối chiếu tài sản phòng {checklist.RoomId}, giường {checklist.BedId}. Số tiền bồi thường nhập theo từng tài sản.",Dock=DockStyle.Fill,TextAlign=ContentAlignment.MiddleLeft};
        var add=Button("Thêm tài sản",AddAssetRow);
        var row1=new FlowLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(4)};row1.Controls.AddRange([_inspected,add]);
        var row2=new FlowLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(4)};row2.Controls.AddRange([_collected,_compensationTotal,new Label{Text="Phương thức thu:",AutoSize=true,Padding=new Padding(0,7,0,0)},_method]);
        var buttons=new FlowLayoutPanel{FlowDirection=FlowDirection.RightToLeft,Dock=DockStyle.Fill,Padding=new Padding(4)};
        var save=new Button{Text=title,AutoSize=true};var cancel=new Button{Text="Hủy",DialogResult=DialogResult.Cancel};buttons.Controls.AddRange([cancel,save]);
        var layout=new TableLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(10),ColumnCount=1,RowCount=6};layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute,36));layout.RowStyles.Add(new RowStyle(SizeType.Absolute,34));layout.RowStyles.Add(new RowStyle(SizeType.Percent,100));layout.RowStyles.Add(new RowStyle(SizeType.Absolute,34));layout.RowStyles.Add(new RowStyle(SizeType.Absolute,38));layout.RowStyles.Add(new RowStyle(SizeType.Absolute,42));
        layout.Controls.Add(prompt,0,0);layout.Controls.Add(_baselineNotice,0,1);layout.Controls.Add(_assets,0,2);layout.Controls.Add(row1,0,3);
        var noteRow=new FlowLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(4)};noteRow.Controls.AddRange([new Label{Text="Ghi chú:",AutoSize=true,Padding=new Padding(0,7,0,0)},_note]);layout.Controls.Add(noteRow,0,4);layout.Controls.Add(row2,0,5);
        var footer=new Panel{Dock=DockStyle.Bottom,Height=46};footer.Controls.Add(buttons);Controls.Add(layout);Controls.Add(footer);
        save.Click+=(_,_)=>
        {
            _assets.EndEdit();
            if(!_inspected.Checked){MessageBox.Show(this,"Hãy xác nhận đã đối chiếu tài sản với biên bản.","Bàn giao",MessageBoxButtons.OK,MessageBoxIcon.Warning);return;}
            if(!TryReadItems(out var items))return;
            var hasDamage=items.Any(HasDamage);var total=items.Sum(x=>x.Compensation);
            if(hasDamage&&(total<=0||!_collected.Checked||(string.IsNullOrWhiteSpace(Note)&&items.All(x=>string.IsNullOrWhiteSpace(x.Note))))){MessageBox.Show(this,"Có thiếu/hư hỏng: nhập tiền bồi thường cho tài sản, mô tả và xác nhận đã lập biên bản, thực thu.","Bàn giao",MessageBoxButtons.OK,MessageBoxIcon.Warning);return;}
            if(!hasDamage&&total>0){MessageBox.Show(this,"Chỉ nhập tiền bồi thường cho tài sản bị thiếu hoặc hư hỏng.","Bàn giao",MessageBoxButtons.OK,MessageBoxIcon.Warning);return;}
            Items=items;DialogResult=DialogResult.OK;Close();
        };
        CancelButton=cancel;
        UpdateCompensationState();
    }

    private static Button Button(string text,Action action){var button=new Button{Text=text,AutoSize=true};button.Click+=(_,_)=>action();return button;}
    private void BuildAssetGrid(IReadOnlyList<HandoverAssetLine> items,bool fixedBaseline)
    {
        _assets.Columns.Add(new DataGridViewTextBoxColumn{Name="Name",HeaderText="Tài sản",FillWeight=145});
        _assets.Columns.Add(new DataGridViewTextBoxColumn{Name="Given",HeaderText="SL giao",FillWeight=55});
        _assets.Columns.Add(new DataGridViewTextBoxColumn{Name="Now",HeaderText="SL thực tế",FillWeight=65});
        _assets.Columns.Add(ConditionColumn("GivenCondition","Tình trạng giao"));
        _assets.Columns.Add(ConditionColumn("NowCondition","Tình trạng hiện tại"));
        _assets.Columns.Add(new DataGridViewTextBoxColumn{Name="Compensation",HeaderText="Bồi thường (đ)",FillWeight=85});
        _assets.Columns.Add(new DataGridViewTextBoxColumn{Name="Note",HeaderText="Ghi chú",FillWeight=125});
        _assets.CellValueChanged+=(_,_)=>UpdateCompensationState();_assets.RowsRemoved+=(_,_)=>UpdateCompensationState();
        _assets.DataError+=(_,e)=>e.ThrowException=false;
        foreach(var item in items)
        {
            var index=_assets.Rows.Add(item.Name,item.QuantityGiven,item.QuantityNow,item.ConditionGiven,item.ConditionNow,item.Compensation,item.Note??"");
            if(fixedBaseline){_assets.Rows[index].Cells[1].ReadOnly=true;_assets.Rows[index].Cells[3].ReadOnly=true;}
        }
    }
    private static DataGridViewComboBoxColumn ConditionColumn(string name,string header)
    {
        var column=new DataGridViewComboBoxColumn{Name=name,HeaderText=header,FillWeight=78,DisplayStyle=DataGridViewComboBoxDisplayStyle.DropDownButton};
        column.Items.AddRange("Tốt","Hư hỏng","Mất","Bảo trì");return column;
    }
    private void AddAssetRow()
    {
        var index=_assets.Rows.Add("",0,0,"Tốt","Tốt",0m,"");
        _assets.CurrentCell=_assets.Rows[index].Cells[0];_assets.BeginEdit(true);
    }
    private bool TryReadItems(out List<HandoverAssetLine> items)
    {
        items=[];
        foreach(DataGridViewRow row in _assets.Rows)
        {
            var name=Convert.ToString(row.Cells[0].Value)?.Trim();if(string.IsNullOrEmpty(name))continue;
            if(!int.TryParse(Convert.ToString(row.Cells[1].Value),out var given)||!int.TryParse(Convert.ToString(row.Cells[2].Value),out var now))
            {MessageBox.Show(this,$"Số lượng giao/trả của '{name}' phải là số nguyên.","Bàn giao",MessageBoxButtons.OK,MessageBoxIcon.Warning);return false;}
            var compensationText=Convert.ToString(row.Cells[5].Value);
            if(!string.IsNullOrWhiteSpace(compensationText)&&!decimal.TryParse(compensationText,NumberStyles.Number,CultureInfo.CurrentCulture,out _))
            {MessageBox.Show(this,$"Tiền bồi thường của '{name}' không hợp lệ.","Bàn giao",MessageBoxButtons.OK,MessageBoxIcon.Warning);return false;}
            var compensation=string.IsNullOrWhiteSpace(compensationText)?0:decimal.Parse(compensationText!,NumberStyles.Number,CultureInfo.CurrentCulture);
            items.Add(new HandoverAssetLine(name,given,now,Convert.ToString(row.Cells[3].Value)??"Tốt",Convert.ToString(row.Cells[4].Value)??"Tốt",compensation,Convert.ToString(row.Cells[6].Value)));
        }
        return true;
    }
    private static bool HasDamage(HandoverAssetLine item)=>item.QuantityNow<item.QuantityGiven||item.ConditionNow=="Mất"||(item.ConditionNow=="Hư hỏng"&&item.ConditionGiven!="Hư hỏng")||item.Compensation>0;
    private void UpdateCompensationState()
    {
        if(IsDisposed)return;
        var total=0m;var damaged=false;
        foreach(DataGridViewRow row in _assets.Rows)
        {
            if(row.IsNewRow)continue;
            var name=Convert.ToString(row.Cells[0].Value)?.Trim();
            int.TryParse(Convert.ToString(row.Cells[1].Value),out var given);int.TryParse(Convert.ToString(row.Cells[2].Value),out var now);
            var givenState=Convert.ToString(row.Cells[3].Value)??"Tốt";var nowState=Convert.ToString(row.Cells[4].Value)??"Tốt";
            decimal.TryParse(Convert.ToString(row.Cells[5].Value),NumberStyles.Number,CultureInfo.CurrentCulture,out var amount);total+=amount;
            if(!string.IsNullOrEmpty(name)&&(now<given||nowState=="Mất"||(nowState=="Hư hỏng"&&givenState!="Hư hỏng")||amount>0))damaged=true;
        }
        _compensationTotal.Text=$"Tổng bồi thường: {total:N0} đ";
        _collected.Enabled=damaged;_method.Enabled=damaged;
        if(!damaged)_collected.Checked=false;
    }
}

internal sealed class HandoverHistoryDialog:Form
{
    public HandoverHistoryDialog(string history)
    {
        Text="Lịch sử hợp đồng và biên bản";Width=850;Height=620;StartPosition=FormStartPosition.CenterParent;
        var content=new TextBox{Multiline=true,ReadOnly=true,ScrollBars=ScrollBars.Vertical,Dock=DockStyle.Fill,WordWrap=true,Font=new Font("Segoe UI",10),Text=history};
        var close=new Button{Text="Đóng",DialogResult=DialogResult.OK,AutoSize=true};var bar=new FlowLayoutPanel{Dock=DockStyle.Bottom,Height=42,FlowDirection=FlowDirection.RightToLeft,Padding=new Padding(6)};bar.Controls.Add(close);
        Controls.Add(content);Controls.Add(bar);AcceptButton=close;
    }
}
