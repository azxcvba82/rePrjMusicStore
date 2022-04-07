﻿//-----------------------------------------------------------------------
// <copyright file= "IdentityUser.cs">
//     Copyright (c) Danvic.Wang All rights reserved.
// </copyright>
// Author: Danvic.Wang
// Created DateTime: 2019/7/21 11:48:21
// Modified by:
// Description: 用户实体
//-----------------------------------------------------------------------
using System;

namespace Music_store_backend.Entities.Permission
{
    public class IdentityUser : EntityBase
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string fNickName { get; set; }

        /// <summary>
        /// 账户
        /// </summary>
        public string fAccount { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string fPassword { get; set; }

        /// <summary>
        /// 加密参数
        /// </summary>
        //public Guid Salt { get; set; }
    }
}