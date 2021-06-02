using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.map.geocode
{
    public class Geocoder
    {
        const string REVERSE_GEOCODING_QUERY = "https://reverse.geocoder.api.here.com/6.2/reversegeocode.json?app_id={0}&app_code={1}&mode=retrieveAddresses&{2}";
        const string PROX = "prox={0}";
        public Location UserLocation { get; set; }
        public Bounds UserMapView { get; set; }
        private string _appId;
        private string _appCode;
        public int? MaxResults { get; set; }
        public IWebProxy Proxy { get; set; }
        public void InitializeMapGeocoder(string appId, string appCode)
        {
            if (string.IsNullOrWhiteSpace(appId))
            {
                throw new ArgumentException("appId can not be null or empty");
            }

            if (string.IsNullOrWhiteSpace(appCode))
            {
                throw new ArgumentException("appCode can not be null or empty");
            }

            this._appId = appId;
            this._appCode = appCode;
        }

        public async Task<string> ReverseGeocodeAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
        {
            try
            {
                string url = GetQueryUrl(latitude, longitude);
                var response = await GetResponse(url, cancellationToken).ConfigureAwait(false);
                string address = response.View[0].Result[0].Location.Address.Label;
                return address;
            }
            catch (Exception ex)
            {
                throw new HereGeocodingException(ex);
            }
        }
        private HttpClient BuildClient()
        {
            if (this.Proxy == null)
            {
                return new HttpClient();
            }

            var handler = new HttpClientHandler { Proxy = this.Proxy };
            return new HttpClient(handler);
        }
        private async Task<Json.Response> GetResponse(string queryURL, CancellationToken cancellationToken)
        {
            using (var client = BuildClient())
            {
                var response = await client.SendAsync(CreateRequest(queryURL), cancellationToken).ConfigureAwait(false);
                using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    var jsonSerializer = new DataContractJsonSerializer(typeof(Json.ServerResponse));
                    var serverResponse = (Json.ServerResponse)jsonSerializer.ReadObject(stream);

                    return serverResponse.ErrorType != null
                        ? throw new HereGeocodingException(serverResponse.Details, serverResponse.ErrorType, serverResponse.ErrorType)
                        : serverResponse.Response;
                }
            }
        }
        private HttpRequestMessage CreateRequest(string url) => new HttpRequestMessage(HttpMethod.Get, url);

        private string GetQueryUrl(double latitude, double longitude)
        {
            var parameters = new StringBuilder();
            bool first = AppendParameter(parameters, string.Format(CultureInfo.InvariantCulture, "{0},{1}", latitude, longitude), PROX, true);
            AppendGlobalParameters(parameters, first);

            return string.Format(REVERSE_GEOCODING_QUERY, _appId, _appCode, parameters.ToString());
        }

        private bool AppendParameter(StringBuilder sb, string parameter, string format, bool first)
        {
            if (!string.IsNullOrEmpty(parameter))
            {
                if (!first)
                {
                    sb.Append('&');
                }
                sb.Append(string.Format(format, UrlEncode(parameter)));
                return false;
            }
            return first;
        }
        private bool AppendGlobalParameters(StringBuilder parameters, bool first)
        {
            var values = GetGlobalParameters().ToArray();

            if (!first)
            {
                parameters.Append("&");
            }

            parameters.Append(BuildQueryString(values));

            return first && !values.Any();
        }
        private string BuildQueryString(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var builder = new StringBuilder();
            foreach (var pair in parameters)
            {
                if (builder.Length > 0)
                {
                    builder.Append("&");
                }

                builder.Append(UrlEncode(pair.Key));
                builder.Append("=");
                builder.Append(UrlEncode(pair.Value));
            }
            return builder.ToString();
        }
        private string UrlEncode(string toEncode) => string.IsNullOrEmpty(toEncode) ? string.Empty : WebUtility.UrlEncode(toEncode);
        private IEnumerable<KeyValuePair<string, string>> GetGlobalParameters()
        {
            if (UserLocation != null)
            {
                yield return new KeyValuePair<string, string>("prox", UserLocation.ToString());
            }

            if (UserMapView != null)
            {
                yield return new KeyValuePair<string, string>("mapview", string.Concat(UserMapView.SouthWest.ToString(), ",", UserMapView.NorthEast.ToString()));
            }

            if (MaxResults != null && MaxResults.Value > 0)
            {
                yield return new KeyValuePair<string, string>("maxresults", MaxResults.Value.ToString(CultureInfo.InvariantCulture));
            }
        }


    }


}
