using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa.Infrastructure;
using QuanLyKyTucXa.Infrastructure.Data.Context;
using QuanLyKyTucXa.Infrastructure.Security;
using QuanLyKyTucXa.Infrastructure.Services;

namespace QuanLyKyTucXa.WinForms.Forms.Rooms;

public sealed class FrmToaNha : Form
{
    private readonly string _connectionString;
    private readonly AuthSession? _session;
    private readonly DataGridView _grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false };
    private readonly FlowLayoutPanel _buttons = new() { Dock = DockStyle.Top, Height = 44, Padding = new Padding(8) };
    public event EventHandler? ReturnHomeRequested;
    private RoomOperationsService Service => new(() => AppDbContextFactory.Create(_connectionString), _session);
    public FrmToaNha(string connectionString, AuthSession? session) { _connectionString=connectionString; _session=session; Text="Quản lý tòa nhà"; BuildUi(); Load += async (_,_)=>await ReloadAsync(); }
    public Task RefreshViewAsync() => ReloadAsync();
    private void BuildUi()
    {
        Width=900; Height=600; StartPosition=FormStartPosition.CenterParent;
        _grid.DataBindingComplete += (_,_) => SetGridHeaders();
        var add=Button("Thêm", async()=>await EditAsync(null)); var edit=Button("Sửa", async()=>{var row=Selected(); if(row is not null) await EditAsync(row);});
        var del=Button("Xóa", async()=>{var row=Selected(); if(row is null)return; if(MessageBox.Show(this,$"Xóa tòa {row.MaToa}?","Xác nhận",MessageBoxButtons.YesNo,MessageBoxIcon.Warning)!=DialogResult.Yes)return; try{await Service.DeleteTowerAsync(row.MaToa);await ReloadAsync();}catch(Exception ex){ShowError(ex);} });
        var refresh=Button("Làm mới", async()=>await ReloadAsync()); var close=Button("Đóng",()=>ReturnHomeRequested?.Invoke(this,EventArgs.Empty));
        add.Visible=edit.Visible=AuthPolicy.Can(_session,AppPermission.ConfigureRooms); del.Visible=AuthPolicy.Can(_session,AppPermission.DeleteRooms);
        _buttons.Controls.AddRange([add,edit,del,refresh,close]); Controls.Add(_grid); Controls.Add(_buttons);
    }
    private void SetGridHeaders()
    {
        SetHeader("MaToa", "Mã tòa"); SetHeader("TenToa", "Tên tòa"); SetHeader("LoaiToa", "Loại tòa");
        SetHeader("SoTang", "Số tầng"); SetHeader("SoPhong", "Số phòng"); SetHeader("TrangThai", "Trạng thái"); SetHeader("GhiChu", "Ghi chú");
        if (_grid.Columns["Phongs"] is { } rooms) rooms.Visible = false;
        SetColumnWeight("MaToa", 10); SetColumnWeight("TenToa", 23); SetColumnWeight("LoaiToa", 12);
        SetColumnWeight("SoTang", 8); SetColumnWeight("SoPhong", 9); SetColumnWeight("TrangThai", 14); SetColumnWeight("GhiChu", 24);
        if (_grid.Columns["GhiChu"] is { } note) note.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        _grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
    }
    private void SetHeader(string property, string title) { if (_grid.Columns[property] is { } column) column.HeaderText = title; }
    private void SetColumnWeight(string property, float weight) { if (_grid.Columns[property] is { } column) column.FillWeight = weight; }
    private static Button Button(string text, Action action) { var b=new Button{Text=text,AutoSize=true,Height=30}; b.Click+=(_,_)=>action(); return b; }
    private ToaNha? Selected()=>_grid.CurrentRow?.DataBoundItem as ToaNha;
    private async Task ReloadAsync(){try{_grid.DataSource=await Service.GetTowersAsync();}catch(Exception ex){ShowError(ex);}}
    private async Task EditAsync(ToaNha? current)
    {
        using var dialog=new TowerEditDialog(current);
        if(dialog.ShowDialog(this)!=DialogResult.OK)return;
        try{await Service.SaveTowerAsync(dialog.Value,current is null); await ReloadAsync();}
        catch(Exception ex){ShowError(ex);}
    }
    private void ShowError(Exception ex)=>MessageBox.Show(this,ex.Message,"Tòa nhà",MessageBoxButtons.OK,MessageBoxIcon.Warning);
}

internal sealed class TowerEditDialog : Form
{
    private readonly TextBox _code=new(),_name=new(),_note=new(); private readonly ComboBox _type=new(){DropDownStyle=ComboBoxStyle.DropDownList}; private readonly ComboBox _status=new(){DropDownStyle=ComboBoxStyle.DropDownList}; private readonly NumericUpDown _floors=new(){Minimum=1,Maximum=100};
    public ToaNha Value {get;}
    public TowerEditDialog(ToaNha? tower)
    {
        Value=tower is null?new ToaNha{TrangThai="Hoạt động",LoaiToa="Hỗn hợp",SoTang=1}:new ToaNha{MaToa=tower.MaToa,TenToa=tower.TenToa,LoaiToa=tower.LoaiToa,SoTang=tower.SoTang,TrangThai=tower.TrangThai,GhiChu=tower.GhiChu};
        Text=tower is null?"Thêm tòa":"Sửa tòa"; Width=440; Height=330; StartPosition=FormStartPosition.CenterParent; FormBorderStyle=FormBorderStyle.FixedDialog; MaximizeBox=false; MinimizeBox=false;
        var layout=new TableLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(14),ColumnCount=2,RowCount=7}; layout.ColumnStyles.Add(new(SizeType.Absolute,120));layout.ColumnStyles.Add(new(SizeType.Percent,100));
        for(var row=0;row<6;row++)layout.RowStyles.Add(new RowStyle(SizeType.Absolute,34));layout.RowStyles.Add(new RowStyle(SizeType.Percent,100));
        _type.Items.AddRange(["Nam","Nữ","Hỗn hợp"]);_status.Items.AddRange(["Hoạt động","Bảo trì","Dừng hoạt động"]);
        Add(layout,"Mã tòa",_code,0);Add(layout,"Tên tòa",_name,1);Add(layout,"Loại tòa",_type,2);Add(layout,"Số tầng",_floors,3);Add(layout,"Trạng thái",_status,4);Add(layout,"Ghi chú",_note,5);
        _code.Text=Value.MaToa;_code.ReadOnly=tower is not null;_name.Text=Value.TenToa;_type.SelectedItem=Value.LoaiToa;_floors.Value=Math.Clamp(Value.SoTang,1,100);_status.SelectedItem=Value.TrangThai;_note.Text=Value.GhiChu;
        var save=new Button{Text="Lưu",DialogResult=DialogResult.OK};var cancel=new Button{Text="Hủy",DialogResult=DialogResult.Cancel};var buttons=new FlowLayoutPanel{Dock=DockStyle.Fill,FlowDirection=FlowDirection.RightToLeft};buttons.Controls.AddRange([cancel,save]);layout.Controls.Add(buttons,1,6);Controls.Add(layout);AcceptButton=save;CancelButton=cancel;
        save.Click+=(_,e)=>{Value.MaToa=_code.Text.Trim();Value.TenToa=_name.Text.Trim();Value.LoaiToa=_type.Text;Value.SoTang=(int)_floors.Value;Value.TrangThai=_status.Text;Value.GhiChu=_note.Text;};
    }
    private static void Add(TableLayoutPanel p,string label,Control c,int row){p.Controls.Add(new Label{Text=label,AutoSize=true,Anchor=AnchorStyles.Left},0,row);c.Dock=DockStyle.Fill;p.Controls.Add(c,1,row);}
}
