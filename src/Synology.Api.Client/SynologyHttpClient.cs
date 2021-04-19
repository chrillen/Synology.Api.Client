﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl.Http;
using Synology.Api.Client.ApiDescription;
using Synology.Api.Client.Exceptions;
using Synology.Api.Client.Session;
using Synology.Api.Client.Shared.Models;

namespace Synology.Api.Client
{
    public class SynologyHttpClient : ISynologyHttpClient
    {
        private readonly IFlurlClient _flurlClient;

        public SynologyHttpClient(IFlurlClient flurlClient)
        {
            _flurlClient = flurlClient;
        }

        public async Task<T> GetAsync<T>(
            IApiInfo apiInfo,
            string apiMethod,
            object queryParams,
            ISynologySession session = null)
        {
            var flurlRequest = BuildGetRequest(apiInfo, apiMethod, queryParams, session);

            using (var httpResponse = await flurlRequest.GetAsync())
            {
                return await HandleSynologyResponse<T>(httpResponse, apiInfo, apiMethod);
            }
        }

        public async Task<T> PostAsync<T>(IApiInfo apiInfo, string apiMethod, HttpContent content, ISynologySession session = null)
        {
            var flurlRequest = _flurlClient.Request(apiInfo.Path);

            if (session != null)
            {
                flurlRequest.SetQueryParam("_sid", session.Sid);
            }

            using (var httpResponse = await flurlRequest.PostAsync(content))
            {
                return await HandleSynologyResponse<T>(httpResponse, apiInfo, apiMethod);
            }
        }

        private IFlurlRequest BuildGetRequest(IApiInfo apiInfo, string apiMethod, object queryParams, ISynologySession session = null)
        {
            var flurlRequest = _flurlClient
                .Request(apiInfo.Path)
                .SetQueryParams(new
                {
                    api = apiInfo.Name,
                    version = apiInfo.Version,
                    method = apiMethod,
                });

            flurlRequest.SetQueryParams(queryParams);

            if (!string.IsNullOrWhiteSpace(apiInfo.SessionName))
            {
                flurlRequest.SetQueryParam("session", apiInfo.SessionName);
            }

            if (session != null)
            {
                flurlRequest.SetQueryParam("_sid", session.Sid);
            }

            return flurlRequest;
        }

        private async Task<T> HandleSynologyResponse<T>(IFlurlResponse httpResponse, IApiInfo apiInfo, string apiMethod)
        {
            switch (httpResponse.StatusCode)
            {
                case (int)HttpStatusCode.OK:
                    var response = await httpResponse.GetJsonAsync<ApiResponse<T>>();

                    if (!response.Success)
                    {
                        throw new SynologyApiException(apiInfo, apiMethod, response.Error.Code);
                    }

                    if (typeof(T) == typeof(BaseApiResponse))
                    {
                        return (T)Activator.CreateInstance(typeof(T), new object[] { response.Success });
                    }

                    return response.Data;

                default:
                    throw new UnexpectedResponseStatusException((HttpStatusCode)httpResponse.StatusCode); ;
            }
        }
    }
}
