using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace CORE_API.Models.CardActivation
{
  public class _WebClientBase
    {
        public string _BaseURL = "https://apimash-uat.network.ae/KMPY/PCServices/";
        public HttpClient client = new HttpClient();
    }
}