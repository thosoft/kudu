﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Kudu.Client.Infrastructure;
using Kudu.Contracts.SiteExtensions;
using Newtonsoft.Json.Linq;

namespace Kudu.Client.SiteExtensions
{
    public class RemoteSiteExtensionManager : KuduRemoteClientBase
    {
        public RemoteSiteExtensionManager(string serviceUrl, ICredentials credentials = null, HttpMessageHandler handler = null)
            : base(serviceUrl, credentials, handler)
        {
        }

        public async Task<IEnumerable<SiteExtensionInfo>> GetRemoteExtensions(string filter = null, bool allowPrereleaseVersions = false, string feedUrl = null)
        {
            var url = new StringBuilder(ServiceUrl);
            url.Append("extensionfeed");

            var separator = '?';
            if (!String.IsNullOrEmpty(filter))
            {
                url.Append(separator);
                url.Append("filter=");
                url.Append(filter);
                separator = '&';
            }

            if (allowPrereleaseVersions)
            {
                url.Append(separator);
                url.Append("allowPrereleaseVersions=");
                url.Append(true);
                separator = '&';
            }

            if (!string.IsNullOrWhiteSpace(feedUrl))
            {
                url.Append(separator);
                url.Append("feedUrl=");
                url.Append(feedUrl);
            }

            return await Client.GetJsonAsync<IEnumerable<SiteExtensionInfo>>(url.ToString());
        }

        public async Task<SiteExtensionInfo> GetRemoteExtension(string id, string version = null, string feedUrl = null)
        {
            var url = new StringBuilder(ServiceUrl);
            url.Append("extensionfeed/");
            url.Append(id);

            var separator = '?';
            if (!String.IsNullOrEmpty(version))
            {
                url.Append(separator);
                url.Append("version=");
                url.Append(version);
                separator = '&';
            }

            if (!string.IsNullOrWhiteSpace(feedUrl))
            {
                url.Append(separator);
                url.Append("feedUrl=");
                url.Append(feedUrl);
            }

            return await Client.GetJsonAsync<SiteExtensionInfo>(url.ToString());
        }

        public async Task<IEnumerable<SiteExtensionInfo>> GetLocalExtensions(string filter = null, bool checkLatest = true)
        {
            var url = new StringBuilder(ServiceUrl);
            url.Append("siteextensions");

            var separator = '?';
            if (!String.IsNullOrEmpty(filter))
            {
                url.Append(separator);
                url.Append("filter=");
                url.Append(filter);
                separator = '&';
            }

            if (checkLatest)
            {
                url.Append(separator);
                url.Append("checkLatest=");
                url.Append(checkLatest);
                separator = '&';
            }

            return await Client.GetJsonAsync<IEnumerable<SiteExtensionInfo>>(url.ToString());
        }

        public async Task<SiteExtensionInfo> GetLocalExtension(string id, bool checkLatest = true)
        {
            var url = new StringBuilder(ServiceUrl);
            url.Append("siteextensions/");
            url.Append(id);

            if (checkLatest)
            {
                url.Append("?checkLatest=");
                url.Append(checkLatest);
            }

            return await Client.GetJsonAsync<SiteExtensionInfo>(url.ToString());
        }

        public async Task<SiteExtensionInfo> InstallExtension(string id, string version = null, string feedUrl = null)
        {
            var json = new JObject();
            json["version"] = version;
            json["feed_url"] = feedUrl;

            return await Client.PutJsonAsync<JObject, SiteExtensionInfo>("siteextensions/" + id, json);
        }

        public async Task<bool> UninstallExtension(string id)
        {
            var url = new StringBuilder(ServiceUrl);
            url.Append("siteextensions/");
            url.Append(id);

            HttpResponseMessage result = await Client.DeleteAsync(new Uri(url.ToString()));
            return await result.EnsureSuccessful().Content.ReadAsAsync<bool>();
        }
    }
}