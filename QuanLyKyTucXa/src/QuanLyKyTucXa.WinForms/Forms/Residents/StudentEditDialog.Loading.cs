using QuanLyKyTucXa.Infrastructure.Services;

namespace QuanLyKyTucXa.WinForms.Forms.Residents;

public partial class StudentEditDialog
{
    public StudentEditDialog(SinhVienService service,string id) : this()
    {
        _service=service;
        Text="Sửa sinh viên";
        btnLuu.Enabled=tlpFields.Enabled=false;
        lblError.Text="Đang tải hồ sơ sinh viên...";
        Shown+=async(_,_)=>
        {
            try
            {
                var value=await Task.Run(()=>service.GetAsync(id));
                if(IsDisposed||Disposing)return;
                BindValue(value,false,service);
                lblError.Text="";
                tlpFields.Enabled=true;
                await LoadAcademicCatalogAsync();
            }
            catch(Exception ex)
            {
                if(!IsDisposed&&!Disposing)lblError.Text=ex is UnauthorizedAccessException or InvalidOperationException?ex.Message:"Không tải được hồ sơ. Đóng cửa sổ và thử lại.";
            }
        };
    }
}
