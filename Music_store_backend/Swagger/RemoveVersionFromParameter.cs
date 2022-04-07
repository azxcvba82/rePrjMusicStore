//-----------------------------------------------------------------------
// <copyright file= "RemoveVersionFromParameter.cs">
//     Copyright (c) Danvic712. All rights reserved.
// </copyright>
// Author: Danvic712
// Created DateTime: 2019/4/6 21:34:05 
// Modified by:
// Description: 删除参数中的 api 版本参数
//-----------------------------------------------------------------------
//using Microsoft.OpenApi.Models;
//using Swashbuckle.AspNetCore.Swagger;
//using Swashbuckle.AspNetCore.SwaggerGen;
//using Swashbuckle.Swagger;
using System.Linq;


namespace Music_store_backend.Swagger
{

    //public class SetVersionInPathDocumentFilter : IDocumentFilter
    //{
    //    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    //    {
    //        var updatedPaths = new OpenApiPaths();

    //        foreach (var entry in swaggerDoc.Paths)
    //        {
    //            updatedPaths.Add(
    //                entry.Key.Replace("v{version}", swaggerDoc.Info.Version),
    //                entry.Value);
    //        }

    //        swaggerDoc.Paths = updatedPaths;
    //    }
    //}

    //public class RemoveVersionFromParameter : IOperationFilter
    //{
    //    void IOperationFilter.Apply(OpenApiOperation operation, OperationFilterContext context)
    //    {
    //        var versionParameter = operation.Parameters.Single(p => p.Name == "version");
    //        operation.Parameters.Remove(versionParameter);
    //    }

    //}
}