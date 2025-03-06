﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.BLL.DTOs.AccountDTOs
{
    public class  AccountChangePassDTO
    {
        [Required(ErrorMessage = "Mật khẩu cũ không được để trống.")]
        public string OldPassword { get; set; } = null!;
        [Required(ErrorMessage = "Mật khẩu mới không đúng định dạng.")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Nhắc lại mật khẩu không được để trống.")]
        [Compare("NewPassword", ErrorMessage = "Nhắc lại mật khẩu không khớp với mật khẩu.")]
        public string ConfirmedNewPassword { get; set; } = null!;
    }
}
