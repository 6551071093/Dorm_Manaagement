using QuanLyKyTucXa.Infrastructure.Services;

namespace QuanLyKyTucXa.WinForms.Forms.Users;

public partial class UserAccountEditDialog
{
    public UserAccountEditDialog(NguoiDungService service,int? id) : this()
    {
        _service=service;
        Text=id is null?"Thêm tài khoản":"Sửa tài khoản";
        btnSave.Enabled=tlpFields.Enabled=false;
        lblMessage.Text="Đang tải thông tin tài khoản...";
        Shown+=async(_,_)=>
        {
            try
            {
                var data=await Task.Run(async()=>
                {
                    var value=id is null?Task.FromResult(new UserAccountEdit()):service.GetAsync(id.Value);
                    var choices=service.StudentChoicesAsync(id);
                    await Task.WhenAll(value,choices);
                    return (Value:await value,Choices:await choices);
                });
                if(IsDisposed||Disposing)return;
                BindValue(data.Value,data.Choices,service);
                btnSave.Enabled=tlpFields.Enabled=true;
            }
            catch(Exception ex)
            {
                if(!IsDisposed&&!Disposing)lblMessage.Text=ex is UnauthorizedAccessException or InvalidOperationException?ex.Message:"Không tải được tài khoản. Đóng cửa sổ và thử lại.";
            }
        };
    }
}
