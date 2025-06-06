﻿using ClickFlow.BLL.DTOs.AccountDTOs;
using ClickFlow.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClickFlow.BLL.Helpers.Validations
{
	public class RequiredIfRoleAttribute : ValidationAttribute
	{
		private readonly Role _role;
		private readonly string _propertyName;

		public RequiredIfRoleAttribute(Role role, string propertyName)
		{
			_role = role;
			_propertyName = propertyName;
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var instance = (AccountCreateRequestDTO)validationContext.ObjectInstance;
			if (instance.Role == _role && (value == null || (value is string str && string.IsNullOrWhiteSpace(str))))
			{
				return new ValidationResult($"{_propertyName} không được để trống khi vai trò là {_role}.");
			}
			return ValidationResult.Success;
		}
	}
}
