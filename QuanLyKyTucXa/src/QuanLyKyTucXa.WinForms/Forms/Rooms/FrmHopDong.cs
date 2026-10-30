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
        var history=new Button{Text="Lịch sử / bàn giao",AutoSize=true};history.Click+=(_,_)=>ShowHistory();
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
            await Service.TransferAsync(row.ContractId,dialog.SelectedBed.BedId);await ReloadAsync();
            MessageBox.Show(this,"Đã chuyển phòng. Trạng thái giường và phòng được đồng bộ trong transaction.","Chuyển phòng",MessageBoxButtons.OK,MessageBoxIcon.Information);
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
        using var note=new HandoverDialog();if(note.ShowDialog(this)!=DialogResult.OK)return;
        try{await Service.CheckoutAsync(row.ContractId,note.Note,note.AssetsInspected,note.DamageReported,note.CompensationCollected,note.CompensationAmount,note.PaymentMethod);await ReloadAsync();MessageBox.Show(this,"Đã thanh lý hợp đồng và cập nhật giường trống.","Trả phòng",MessageBoxButtons.OK,MessageBoxIcon.Information);}
        catch(Exception ex){Error(ex);}
    }
    private void ShowHistory()
    {
        var row=Selected();if(row is null)return;
        var history=string.IsNullOrWhiteSpace(row.History)?"Hợp đồng chưa có ghi chú hoặc lịch sử thao tác.":row.History;
        MessageBox.Show(this,$"Hợp đồng: {row.ContractId}\nSinh viên: {row.StudentName} ({row.StudentId})\nPhòng/giường hiện tại: {row.RoomId} / {row.BedId}\n\nLịch sử và bàn giao:\n{history}","Lịch sử hợp đồng",MessageBoxButtons.OK,MessageBoxIcon.Information);
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
    private readonly TextBox _note=new(){MaxLength=180,PlaceholderText="Mô tả hư hỏng/mất mát hoặc ghi chú bàn giao"};
    private readonly CheckBox _inspected=new(){Text="Đã kiểm tra tài sản và thiết bị phòng",AutoSize=true};
    private readonly CheckBox _damage=new(){Text="Có hư hỏng/mất mát cần bồi thường",AutoSize=true};
    private readonly CheckBox _collected=new(){Text="Đã lập biên bản và thực thu khoản bồi thường",AutoSize=true};
    private readonly NumericUpDown _amount=new(){Minimum=0,Maximum=9999999999999999.99m,DecimalPlaces=2,ThousandsSeparator=true,Increment=50000};
    private readonly ComboBox _method=new(){DropDownStyle=ComboBoxStyle.DropDownList};
    public string? Note=>string.IsNullOrWhiteSpace(_note.Text)?null:_note.Text.Trim();
    public bool AssetsInspected=>_inspected.Checked;
    public bool DamageReported=>_damage.Checked;
    public bool CompensationCollected=>_collected.Checked;
    public decimal CompensationAmount=>(decimal)_amount.Value;
    public string PaymentMethod=>_method.Text;
    public HandoverDialog()
    {
        Text="Bàn giao phòng";Width=610;Height=370;StartPosition=FormStartPosition.CenterParent;
        _method.Items.AddRange(["Tiền mặt","Chuyển khoản VietQR","Chuyển khoản ngân hàng"]);_method.SelectedIndex=0;
        var prompt=new Label{Text="Kiểm tra tài sản trước khi trả phòng. Nếu có hư hỏng/mất mát, nhập khoản bồi thường và xác nhận đã thực thu.",AutoSize=true};
        var amountRow=new FlowLayoutPanel{AutoSize=true};amountRow.Controls.AddRange([new Label{Text="Số tiền (đ):",AutoSize=true,Padding=new Padding(0,5,0,0)},_amount,new Label{Text="Phương thức:",AutoSize=true,Padding=new Padding(8,5,0,0)},_method]);
        var save=new Button{Text="Xác nhận trả phòng"};var cancel=new Button{Text="Hủy",DialogResult=DialogResult.Cancel};var buttons=new FlowLayoutPanel{FlowDirection=FlowDirection.RightToLeft,Dock=DockStyle.Fill};buttons.Controls.AddRange([cancel,save]);
        var layout=new TableLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(12),ColumnCount=1,RowCount=7};layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute,44));layout.RowStyles.Add(new RowStyle(SizeType.Absolute,28));layout.RowStyles.Add(new RowStyle(SizeType.Absolute,28));layout.RowStyles.Add(new RowStyle(SizeType.Absolute,36));layout.RowStyles.Add(new RowStyle(SizeType.Absolute,28));layout.RowStyles.Add(new RowStyle(SizeType.Percent,100));layout.RowStyles.Add(new RowStyle(SizeType.Absolute,40));
        layout.Controls.Add(prompt,0,0);layout.Controls.Add(_inspected,0,1);layout.Controls.Add(_damage,0,2);layout.Controls.Add(amountRow,0,3);layout.Controls.Add(_collected,0,4);_note.Dock=DockStyle.Fill;layout.Controls.Add(_note,0,5);layout.Controls.Add(buttons,0,6);Controls.Add(layout);CancelButton=cancel;
        void SetDamageFields(){amountRow.Enabled=_damage.Checked;_collected.Enabled=_damage.Checked;if(!_damage.Checked){_amount.Value=0;_collected.Checked=false;}}
        _damage.CheckedChanged+=(_,_)=>SetDamageFields();SetDamageFields();
        save.Click+=(_,_)=>
        {
            if(!_inspected.Checked){MessageBox.Show(this,"Hãy xác nhận đã kiểm tra tài sản phòng.","Bàn giao",MessageBoxButtons.OK,MessageBoxIcon.Warning);return;}
            if(_damage.Checked&&(_amount.Value<=0||!_collected.Checked||string.IsNullOrWhiteSpace(_note.Text))){MessageBox.Show(this,"Khi có hư hỏng, cần mô tả, nhập số tiền lớn hơn 0 và xác nhận đã lập biên bản, thực thu.","Bàn giao",MessageBoxButtons.OK,MessageBoxIcon.Warning);return;}
            DialogResult=DialogResult.OK;Close();
        };
    }
}
