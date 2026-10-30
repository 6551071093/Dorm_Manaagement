using QuanLyKyTucXa.Infrastructure;
using QuanLyKyTucXa.Infrastructure.Services;

namespace QuanLyKyTucXa.WinForms.Forms.Rooms;

public partial class RoomEditDialog
{
    public RoomEditDialog(PhongGiuongService service, string? id) : this()
    {
        _service=service;
        Text=id is null?"Thêm phòng":"Sửa phòng";
        btnLuu.Enabled=tlpFields.Enabled=false;
        lblError.Text="Đang tải thông tin phòng...";
        Shown+=async(_,_)=>
        {
            try
            {
                var data=await Task.Run(async()=>
                {
                    var towers=service.GetTowersAsync();
                    var value=id is null?Task.FromResult(new Phong()):service.GetAsync(id);
                    await Task.WhenAll(towers,value);
                    return (Towers:await towers,Value:await value);
                });
                if(IsDisposed||Disposing)return;
                if(data.Towers.Count==0){lblError.Text="Chưa có tòa nhà để khai báo phòng.";return;}
                BindValue(data.Value,id is null,data.Towers,service);
                lblError.Text="";
                btnLuu.Enabled=tlpFields.Enabled=true;
            }
            catch(Exception ex)
            {
                if(!IsDisposed&&!Disposing)lblError.Text=ex is UnauthorizedAccessException or InvalidOperationException?ex.Message:"Không tải được phòng. Đóng cửa sổ và thử lại.";
            }
        };
    }
}
