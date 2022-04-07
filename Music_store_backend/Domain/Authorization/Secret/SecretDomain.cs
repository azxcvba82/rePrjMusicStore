//-----------------------------------------------------------------------
// <copyright file= "SecretDomain.cs">
//     Copyright (c) Danvic712. All rights reserved.
// </copyright>
// Author: Danvic712
// Created DateTime: 2019/2/17 13:44:12 
// Modified by:
// Description: 
//-----------------------------------------------------------------------
using Dapper;
using Music_store_backend.Entities.Permission;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Music_store_backend.Domain.Authorization.Secret
{
    public class SecretDomain : ISecretDomain
    {
        #region Initialize

        /// <summary>
        /// 仓储接口
        /// </summary>
        //private readonly IDataRepository _repository;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="repository"></param>
        //public SecretDomain(IDataRepository repository)
        //{
        //    _repository = repository;
        //}

        #endregion

        #region API Implements

        /// <summary>
        /// 根据帐户名、密码获取用户实体信息
        /// </summary>
        /// <param name="account">账户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public async Task<IdentityUser> GetUserForLoginAsync(string account, string password)
        {
            StringBuilder strSql = new StringBuilder();
            
            strSql.Append(@"SELECT fNickName, fAccount, fPassword FROM tMember WHERE fAccount = @account AND fPassword = @password;");
            string sql = strSql.ToString();

            IdentityUser members = null;
            using (var conn = new SqlConnection(DataAccessService.connectionStr))
            {
                members = await conn.QueryFirstOrDefaultAsync<IdentityUser>(sql,new {
                    account,
                    password
                });
            }

            return members;


            //return await DBManager.MsSQL.ExecuteAsync<IdentityUser>(sql, new
            //{
            //    account,
            //    password
            //});
        }

        #endregion
    }
}