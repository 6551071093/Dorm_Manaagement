using QuanLyKyTucXa.Infrastructure.Data.Context;
using QuanLyKyTucXa.Infrastructure.Security;
using QuanLyKyTucXa.Infrastructure.Services;

namespace QuanLyKyTucXa.WinForms.Forms.Residents;

public partial class FrmCaNhan : Form
{
    private enum PersonalSection {Rooms,Bills,Incidents}
    private readonly PersonalService? _service;
    private PersonalSection _section;
    private bool _busy,_loaded;
    private string? _loadError;
    public FrmCaNhan()
    {
        InitializeComponent();
        foreach(var grid in new[]{gridRooms,gridBills,gridIncidents})grid.CellFormatting+=PersonalStatusFormatting;
        ShowSection(PersonalSection.Rooms);
        Load+=async(_,_)=>await RefreshViewAsync();
    }
    public FrmCaNhan(string connectionString,AuthSession session):this()=>_service=new PersonalService(()=>AppDbContextFactory.Create(connectionString),session);
    public void SelectSection(string? section)=>ShowSection(section switch{"Hóa đơn"=>PersonalSection.Bills,"Sự cố"=>PersonalSection.Incidents,_=>PersonalSection.Rooms});
    public async Task RefreshViewAsync()
    {
        if(_busy || _service is null)return;
        _busy=true;_loadError=null;btnRefresh.Enabled=btnReport.Enabled=false;UpdateSectionCount();
        try
        {
            var data=await _service.LoadAsync();
            if(IsDisposed||Disposing)return;
            BindProfile(data.Profile);
            gridRooms.DataSource=data.Rooms;
            gridBills.DataSource=data.Bills.Select(b=>new{b.MaHoaDon,b.Thang,b.Nam,b.TongTien,b.DaThu,ConNo=Math.Max(0,b.TongTien-b.DaThu),b.HanThanhToan,b.TrangThai}).ToList();
            gridIncidents.DataSource=data.Incidents;
            _loaded=true;
        }
        catch(Exception ex)
        {
            if(IsDisposed||Disposing)return;
            _loaded=false;_loadError=ex is UnauthorizedAccessException?ex.Message:"Không tải được dữ liệu cá nhân. Kiểm tra kết nối và bấm Làm mới.";
            lblName.Text="Chưa tải được hồ sơ";lblProfile.Text="";avatar.Text="SV";profileStatus.Text="Chưa tải được";
            gridRooms.DataSource=gridBills.DataSource=gridIncidents.DataSource=null;
        }
        finally
        {
            _busy=false;
            if(!IsDisposed&&!Disposing){btnRefresh.Enabled=true;btnReport.Enabled=_loaded;UpdateSectionCount();}
        }
    }
    private void BindProfile(PersonalProfile profile)
    {
        lblName.Text=profile.HoTen;lblProfile.Text=$"{profile.MaSv}  •  {profile.Lop}  •  {profile.Khoa}";
        hints.SetToolTip(lblProfile,lblProfile.Text);hints.SetToolTip(lblName,profile.HoTen);
        var words=profile.HoTen.Split(' ',StringSplitOptions.RemoveEmptyEntries);
        avatar.Text=words.Length==0?"SV":(words[0][..1]+(words.Length>1?words[^1][..1]:"")).ToUpperInvariant();
        profileStatus.Text=profile.TrangThai;
        profileStatus.BackColor=profile.TrangThai=="Đang ở"?Color.FromArgb(232,246,234):profile.TrangThai=="Chờ xếp phòng"?Color.FromArgb(255,243,207):Color.FromArgb(237,242,249);
        profileStatus.ForeColor=profile.TrangThai=="Đang ở"?Color.FromArgb(45,113,78):profile.TrangThai=="Chờ xếp phòng"?Color.FromArgb(155,105,21):Color.FromArgb(80,106,142);
        profileStatus.Invalidate();avatar.Invalidate();
    }
    private void ShowSection(PersonalSection section)
    {
        _section=section;
        gridRooms.Visible=section==PersonalSection.Rooms;gridBills.Visible=section==PersonalSection.Bills;gridIncidents.Visible=section==PersonalSection.Incidents;
        navRooms.Selected=section==PersonalSection.Rooms;navBills.Selected=section==PersonalSection.Bills;navIncidents.Selected=section==PersonalSection.Incidents;
        ActiveGrid.BringToFront();UpdateSectionCount();
    }
    private DataGridView ActiveGrid=>_section switch{PersonalSection.Bills=>gridBills,PersonalSection.Incidents=>gridIncidents,_=>gridRooms};
    private void UpdateSectionCount()
    {
        var unit=_section==PersonalSection.Bills?"hóa đơn":_section==PersonalSection.Incidents?"báo hỏng":"hợp đồng";
        lblMessage.Text=_busy?"Đang tải dữ liệu...":_loadError??$"{ActiveGrid.Rows.Count} {unit} • Chỉ hiển thị dữ liệu của bạn";
        emptyPanel.Visible=_busy || _loadError is not null || ActiveGrid.Rows.Count==0;
        emptySymbol.Section=(int)_section;emptySymbol.Invalidate();
        lblEmptyTitle.Text=_busy?"Đang tải dữ liệu...":_loadError is not null?"Không tải được dữ liệu":_section switch{PersonalSection.Bills=>"Bạn chưa có hóa đơn nào",PersonalSection.Incidents=>"Bạn chưa gửi báo hỏng nào",_=>"Bạn chưa có hợp đồng nào"};
        lblEmptySubtitle.Text=_loadError??(_busy?"Vui lòng chờ trong giây lát.":_section switch{PersonalSection.Bills=>"Hóa đơn của bạn sẽ hiển thị tại đây khi được lập.",PersonalSection.Incidents=>"Khi có sự cố trong phòng, bấm Gửi báo hỏng để thông báo.",_=>"Khi được xếp phòng, thông tin hợp đồng sẽ hiển thị tại đây."});
        LayoutEmptyState();if(emptyPanel.Visible)emptyPanel.BringToFront();
    }
    private async void Refresh_Click(object? sender,EventArgs e)=>await RefreshViewAsync();
    private async void Report_Click(object? sender,EventArgs e)
    {
        if(_busy||_service is null)return;
        using var dialog=new ReportIncidentDialog(_service);
        if(dialog.ShowDialog(this)==DialogResult.OK){ShowSection(PersonalSection.Incidents);await RefreshViewAsync();}
    }
}
