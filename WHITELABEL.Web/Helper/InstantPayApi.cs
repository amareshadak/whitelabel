﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WHITELABEL.Web.Helper
{
    public static class InstantPayApi
    {
        
        public static string root = "https://www.instantpay.in";
        //public static string token = "c87d7fd89da4088d4dbdc597907ffb41";
        //c87d7fd89da4088d4dbdc597907ffb41
        //public static string token = "06dd5d41886312228deda4b309851267";
        public static string token = System.Configuration.ConfigurationManager.AppSettings["InstantPayToken"];
        //4ac2bf68830bd5430b4f475f08a6e2c4
        public static string EdnPointIP = "103.240.90.56";
        public static string MACIP = "00-15-5D-5A-17-3E";
        //public static string EdnPointIP = "103.240.91.190";
        //public static string MACIP = "00-15-5D-5A-17-69";
        //public static string agentId = "74Y104314";
        public static string agentId = System.Configuration.ConfigurationManager.AppSettings["InstantPayAgentID"];
        public static class PaymentAPI
        {
            //public static string Validation(string agentid, string amount, string spkey, string account)
            public static string Validation(string agentid, string amount, string spkey, string account,string Outletid,string CustomerMobileNo)
            {
                try
                {
                    string transactionstatus = string.Empty;
                    var listinfo = "";

                    //https://www.instantpay.in/ws/api/transaction?format=json&token={{token}}&agentid={{agent_id}}&amount={{amount}}&spkey={{spkey}}&account={{account}}&mode=VALIDATE&optional1={{optional1}}&optional2={{optional2}}&optional3={{optional3}}&optional4={{optional4}}&optional5={{optional5}}&optional6={{optional6}}&optional7={{optional7}}&optional8={{optional8}}&optional9={{optional9}}&outletid={{outletid}}&customermobile={{customermobile}}
                    string url = root + "/ws/api/transaction?format=json&token=" + token + "&agentid=" + agentid + "&amount=" + amount + "&spkey=" + spkey + "&account=" + account + "&mode=VALIDATE&optional1={{optional1}}&optional2={{optional2}}&optional3={{optional3}}&optional4={{optional4}}&optional5={{optional5}}&optional6={{optional6}}&optional7={{optional7}}&optional8={{optional8}}&optional9={{optional9}}&outletid=" + Outletid+ "&customermobile=" + CustomerMobileNo+"";
                    //string url = root + "/ws/api/report?format=json&token=" + token + "&agentid=" + agentid + "&amount=" + amount + "&spkey=" + spkey + "&account=" + account + "&mode=VALIDATE&optional1={{optional1}}&optional2={{optional2}}&optional3={{optional3}}&optional4={{optional4}}&optional5={{optional5}}&optional6={{optional6}}&optional7={{optional7}}&optional8={{optional8}}&optional9={{optional9}}&outletid={{outletid}}&endpointip={{customer_ip}}&customermobile={{customermobile}}&paymentmode={{paymentmode}}&paymentchannel={{paymentchannel}}";
                    var res = GetResponse(url, "GET", new Dictionary<string, string>());
                    string errorcode = res.ipay_errorcode;
                    string des = res.ipay_errordesc;
                    if (errorcode == "TXN")
                    {
                        transactionstatus = errorcode;
                    }
                    else
                    {
                        transactionstatus = InstantPayError.GetError(errorcode);
                    }
                    return transactionstatus;
                    //if(res.)
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            //public static dynamic Payment(string agentid, string amount, string spkey, string account)
            public static dynamic Payment(string agentid, string amount, string spkey, string account, string Outletid, string CustomerMobileNo)
            {
                try
                {
                    string APIMsg = string.Empty;
                    //https://www.instantpay.in/ws/api/transaction?format=xml&token={{token}}&spkey={{spkey}}&agentid={{agent_id}}&amount={{amount}}&account={{account}}&optional1={{optional1}}&optional2={{optional2}}&optional3={{optional3}}&optional4={{optional4}}&optional5={{optional5}}&optional6={{optional6}}&optional7={{optional7}}&optional8={{optional8}}&optional9={{optional9}}&outletid={{outletid}}&customermobile={{customermobile}}
                    string url = root + "/ws/api/transaction?format=json&token=" + token + "&spkey=" + spkey + "&agentid=" + agentId + "&amount=" + amount + "&account=" + account + "&optional1={{optional1}}&optional2={{optional2}}&optional3={{optional3}}&optional4={{optional4}}&optional5={{optional5}}&optional6={{optional6}}&optional7={{optional7}}&optional8={{optional8}}&optional9={{optional9}}&outletid={{outletid}}&customermobile=" + CustomerMobileNo + "";
                    //string url = root + "/ws/api/transaction?format=json&token=" + token + "&spkey=" + spkey + "&agentid=" + agentId + "&amount=" + amount + "&account=" + account + "&customermobile=" + CustomerMobileNo + "";
                    var res = GetResponse(url, "GET", new Dictionary<string, string>());
                    string errorcode = res.res_code;//res.res_code;
                    string status = res.status;
                    if (errorcode == "TXN")
                    {
                        //APIMsg = errorcode;
                        string ipay_id = res.ipay_id;
                        //APIMsg = errorcode;
                        return res;
                    }
                    else
                    {
                        //APIMsg = InstantPayError.GetError(res.ipay_errorcode.Value);
                        return res;
                    }
                    //return APIMsg;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }


            public static dynamic InstantPayStatusCheckAPI(string OrderId)
            {
                //string url = root + "/ws/outlet/sendOTP";
                string url = root + "/ws/status/checkbyorderid";/*registrationOTP*/
                var param = new Dictionary<string, dynamic> {
                    {
                        "token", token
                    },
                    {
                        "request", new Dictionary<string, string> {
                            { "order_id", OrderId}
                        }

                    }
                };
                var res = GetResponse(url, "POST", param);
                if (res.statuscode.Value == "TXN")
                {
                    return res;
                }
                else
                {
                    //return InstantPayError.GetError(res.statuscode.Value);
                    return res;
                }
            }

            public static dynamic StatusCheck(string agentid)
            {
                try
                {
                    string APIMsg = string.Empty;
                    string url = root + "/ws/api/getMIS?format=json&token=" + token + "&agentid=" + agentid;
                    var res = GetResponse(url, "GET", new Dictionary<string, string>());
                    string errorcode = res.res_code;
                    if (errorcode == "TXN")
                    {
                        //APIMsg = errorcode;
                        return res;
                    }
                    else
                    {
                        //APIMsg = InstantPayError.GetError(errorcode);
                        return res;
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            public static dynamic ServiceProviderDetails(string spkey)
            {
                try
                {
                    string url = root + "/ws/api/serviceproviders?token=" + token + "&spkey=" + spkey + "&format=json";
                    var res = GetResponse(url, "GET", new Dictionary<string, string>());
                    string errorcode = res.ipay_errorcode;
                    if (errorcode == "TXN")
                    {
                        return res;
                    }
                    else
                    {
                        return InstantPayError.GetError(errorcode);
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

            public static dynamic ComplaintsAPI(string iPayId)
            {
                try
                {
                    string APIMsg = string.Empty;
                    string url = root + "/ws/api/report?format=json&token=" + token + "&ipayid=" + iPayId;
                    var res = GetResponse(url, "GET", new Dictionary<string, string>());
                    string errorcode = res.res_code;
                    if (errorcode == "TXN")
                    {
                        //APIMsg = errorcode;
                        return res;
                    }
                    else
                    {
                        //APIMsg = InstantPayError.GetError(errorcode);
                        return res;
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static class BBPSPaymentAPI
        {
            public static dynamic BBPSServiceProviderDetails(string ServiceKey)
            {
                string url = root + "/ws/userresources/bbps_biller";
                var param = new Dictionary<string, dynamic> {
                    {
                        "token", token
                    },
                    {
                        "request", new Dictionary<string, string> {
                            { "sp_key", ServiceKey}
                        }

                    }
                };
                var res = GetResponse(url, "POST", param);
                if (res.statuscode.Value == "TXN")
                {
                    return res;
                }
                else
                {
                    //return InstantPayError.GetError(res.statuscode.Value);
                    return res;
                }
            }

            public static dynamic BBPSBillPaymentValidation(string SP_Key, string CUS_Mobile, string Cus_Param, string Amount, string latlong, string Outlet,string Ref_ID)
            {
                try
                {
                    string transactionstatus = string.Empty;
                    List<string> objvalcus = new List<string>();
                    objvalcus.Add(CUS_Mobile);
                    objvalcus.Add(Cus_Param);
                    dynamic LandlineObj = new JObject();
                    LandlineObj.token = token;
                    dynamic RequestVal = new JObject();
                    RequestVal.sp_key = SP_Key;
                    RequestVal.agentid = agentId;
                    RequestVal.customer_mobile = CUS_Mobile;
                    RequestVal.customer_params = new JArray(CUS_Mobile,Cus_Param);
                    RequestVal.init_channel = "AGT";
                    RequestVal.endpoint_ip = EdnPointIP;
                    RequestVal.mac = MACIP;
                    RequestVal.payment_mode = "CASH";
                    RequestVal.payment_info = "bill";
                    //RequestVal.payment_info = "12"; //pending
                    //RequestVal.payment_info = "11";  //success
                    RequestVal.amount = Amount;
                    RequestVal.reference_id = "";
                    //RequestVal.latlong = latlong;   //22.584400177001953,88.3311996459961
                    RequestVal.latlong = "22.5844,88.3311";   //22.584400177001953,88.3311996459961
                    RequestVal.outletid = Outlet;
                    LandlineObj.request = new JObject(RequestVal);

                    string SearchparamValue = JsonConvert.SerializeObject(LandlineObj);
                    Dictionary<string, dynamic> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(SearchparamValue);
                    //var jToken = JToken.Parse(LandlineObj);
                    //var users = jToken.ToObject<List<ParameterBBPS>>();
                    //Dictionary<string, string> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(LandlineObj);
                    //var wordDictionary = new Dictionary<string, List<string>>();
                    //wordDictionary.Add(CUS_Mobile);

                    string url = root + "/ws/bbps/bill_fetch";
                //    var param = new Dictionary<string, dynamic> {
                //    {
                //        "token", token
                //    },
                //    {
                //        "request", new Dictionary<string, string> {
                //            { "sp_key", SP_Key},
                //            { "agentid", agentId},
                //            {"customer_mobile", CUS_Mobile},
                //            {"customer_mobile", CUS_Mobile},
                //            {"customer_params", "['','{{}}']"},
                //            { "init_channel","AGT"},
                //            {"endpoint_ip","115.96.143.199" },
                //            {"mac"," 00-0F-22-00-ED-D2" },
                //            {"payment_mode","Cash" },
                //            {"payment_info","Bill" },
                //            {"amount",Amount },
                //            {"reference_id","" },
                //            {"latlong","" },
                //            {"outletid","6601" },
                //        }

                //    }
                //};
                    //var res = GetResponse(url, "POST", param);
                    var res = GetResponse(url, "POST", temp);
                    string errorcode = res.statuscode;
                    string des = res.status;
                    if (errorcode == "TXN")
                    {
                        //transactionstatus = res;
                        return res;
                    }
                    else
                    {
                        //return InstantPayError.GetError(res.statuscode.Value);
                        //transactionstatus = InstantPayError.GetError(errorcode); ;
                        //transactionstatus = des;
                        return res;
                    }
                    //return transactionstatus;
                }
                catch (Exception res)
                {

                    throw;
                }

            }
            
            public static dynamic BBPSBillPaymentPOSTPAID(string SP_Key, string CUS_Mobile, string Cus_Param, string Amount, string latlong, string Outlet,string Ref_ID)
            {
                try
                {
                    List<string> objvalcus = new List<string>();
                    objvalcus.Add(CUS_Mobile);
                    objvalcus.Add(Cus_Param);
                    dynamic LandlineObj = new JObject();
                    LandlineObj.token = token;
                    dynamic RequestVal = new JObject();
                    RequestVal.sp_key = SP_Key;
                    RequestVal.agentid = agentId;
                    RequestVal.customer_mobile = CUS_Mobile;
                    RequestVal.customer_params = new JArray(CUS_Mobile, Cus_Param);
                    RequestVal.init_channel = "AGT";
                    RequestVal.endpoint_ip = EdnPointIP;
                    RequestVal.mac = MACIP;
                    RequestVal.payment_mode = "Cash";
                    RequestVal.payment_info = "bill";
                    RequestVal.amount = Amount;
                    RequestVal.reference_id = Ref_ID;
                    //RequestVal.latlong = latlong.Replace("\"","");
                    RequestVal.latlong = "22.5844,88.3311";
                    RequestVal.outletid = Outlet;
                    LandlineObj.request = new JObject(RequestVal);

                    string SearchparamValue = JsonConvert.SerializeObject(LandlineObj);
                    Dictionary<string, dynamic> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(SearchparamValue);
                    //var jToken = JToken.Parse(LandlineObj);
                    //var users = jToken.ToObject<List<ParameterBBPS>>();
                    //Dictionary<string, string> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(LandlineObj);
                    //var wordDictionary = new Dictionary<string, List<string>>();
                    //wordDictionary.Add(CUS_Mobile);

                    string url = root + "/ws/bbps/bill_pay";
                    //    var param = new Dictionary<string, dynamic> {
                    //    {
                    //        "token", token
                    //    },
                    //    {
                    //        "request", new Dictionary<string, string> {
                    //            { "sp_key", SP_Key},
                    //            { "agentid", agentId},
                    //            {"customer_mobile", CUS_Mobile},
                    //            {"customer_mobile", CUS_Mobile},
                    //            {"customer_params", "['','{{}}']"},
                    //            { "init_channel","AGT"},
                    //            {"endpoint_ip","115.96.143.199" },
                    //            {"mac"," 00-0F-22-00-ED-D2" },
                    //            {"payment_mode","Cash" },
                    //            {"payment_info","Bill" },
                    //            {"amount",Amount },
                    //            {"reference_id","" },
                    //            {"latlong","" },
                    //            {"outletid","6601" },
                    //        }

                    //    }
                    //};
                    //var res = GetResponse(url, "POST", param);
                    var res = GetResponse(url, "POST", temp);
                    string errorcode = res.statuscode;//res.res_code;
                    string status = res.status;
                    if (errorcode == "TXN" || errorcode == "TUP" || errorcode=="ERR")
                    {
                        //APIMsg = errorcode;
                        string ipay_id = res.data.ipay_id;
                        //APIMsg = errorcode;
                        return res;
                    }
                    else
                    {
                        //APIMsg = InstantPayError.GetError(res.ipay_errorcode.Value);
                        return res;
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }

            }
            public static dynamic BBPSBillPaymentLANDLINE(string SP_Key, string CUS_Mobile, string Cus_Param, string Amount, string latlong, string Outlet, string Ref_ID)
            {
                try
                {
                    List<string> objvalcus = new List<string>();
                    objvalcus.Add(CUS_Mobile);
                    objvalcus.Add(Cus_Param);
                    dynamic LandlineObj = new JObject();
                    LandlineObj.token = token;
                    dynamic RequestVal = new JObject();
                    RequestVal.sp_key = SP_Key;
                    RequestVal.agentid = agentId;
                    RequestVal.customer_mobile = CUS_Mobile;
                    RequestVal.customer_params = new JArray(CUS_Mobile, Cus_Param);
                    RequestVal.init_channel = "AGT";
                    RequestVal.endpoint_ip = EdnPointIP;
                    RequestVal.mac = MACIP;
                    RequestVal.payment_mode = "Cash";
                    RequestVal.payment_info = "bill";
                    RequestVal.amount = Amount;
                    RequestVal.reference_id = Ref_ID;
                    //RequestVal.latlong = latlong.Replace("\"", "");
                    RequestVal.latlong = "22.5844,88.3311";
                    RequestVal.outletid = Outlet;
                    LandlineObj.request = new JObject(RequestVal);

                    string SearchparamValue = JsonConvert.SerializeObject(LandlineObj);
                    Dictionary<string, dynamic> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(SearchparamValue);
                    //var jToken = JToken.Parse(LandlineObj);
                    //var users = jToken.ToObject<List<ParameterBBPS>>();
                    //Dictionary<string, string> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(LandlineObj);
                    //var wordDictionary = new Dictionary<string, List<string>>();
                    //wordDictionary.Add(CUS_Mobile);

                    string url = root + "/ws/bbps/bill_pay";
                    //    var param = new Dictionary<string, dynamic> {
                    //    {
                    //        "token", token
                    //    },
                    //    {
                    //        "request", new Dictionary<string, string> {
                    //            { "sp_key", SP_Key},
                    //            { "agentid", agentId},
                    //            {"customer_mobile", CUS_Mobile},
                    //            {"customer_mobile", CUS_Mobile},
                    //            {"customer_params", "['','{{}}']"},
                    //            { "init_channel","AGT"},
                    //            {"endpoint_ip","115.96.143.199" },
                    //            {"mac"," 00-0F-22-00-ED-D2" },
                    //            {"payment_mode","Cash" },
                    //            {"payment_info","Bill" },
                    //            {"amount",Amount },
                    //            {"reference_id","" },
                    //            {"latlong","" },
                    //            {"outletid","6601" },
                    //        }

                    //    }
                    //};
                    //var res = GetResponse(url, "POST", param);
                    var res = GetResponse(url, "POST", temp);
                    string errorcode = res.statuscode;//res.res_code;
                    string status = res.status;
                    if (errorcode == "TXN" || errorcode == "TUP" || errorcode == "ERR")
                    {
                        //APIMsg = errorcode;
                        string ipay_id = res.data.ipay_id;
                        //APIMsg = errorcode;
                        return res;
                    }
                    else
                    {
                        //APIMsg = InstantPayError.GetError(res.ipay_errorcode.Value);
                        return res;
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }

            }

            public static dynamic BBPSBillPaymentBroadBand(string SP_Key, string CUS_Mobile, string Cus_Param, string Amount, string latlong, string Outlet, string Ref_ID)
            {
                try
                {
                    List<string> objvalcus = new List<string>();
                    objvalcus.Add(CUS_Mobile);
                    objvalcus.Add(Cus_Param);
                    dynamic LandlineObj = new JObject();
                    LandlineObj.token = token;
                    dynamic RequestVal = new JObject();
                    RequestVal.sp_key = SP_Key;
                    RequestVal.agentid = agentId;
                    RequestVal.customer_mobile = CUS_Mobile;
                    RequestVal.customer_params = new JArray(CUS_Mobile, Cus_Param);
                    RequestVal.init_channel = "AGT";
                    RequestVal.endpoint_ip = EdnPointIP;
                    RequestVal.mac = MACIP;
                    RequestVal.payment_mode = "Cash";
                    RequestVal.payment_info = "bill";
                    RequestVal.amount = Amount;
                    RequestVal.reference_id = Ref_ID;
                    //RequestVal.latlong = latlong.Replace("\"", "");
                    RequestVal.latlong = "22.5844,88.3311";
                    RequestVal.outletid = Outlet;
                    LandlineObj.request = new JObject(RequestVal);

                    string SearchparamValue = JsonConvert.SerializeObject(LandlineObj);
                    Dictionary<string, dynamic> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(SearchparamValue);
                    //var jToken = JToken.Parse(LandlineObj);
                    //var users = jToken.ToObject<List<ParameterBBPS>>();
                    //Dictionary<string, string> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(LandlineObj);
                    //var wordDictionary = new Dictionary<string, List<string>>();
                    //wordDictionary.Add(CUS_Mobile);

                    string url = root + "/ws/bbps/bill_pay";
                    //    var param = new Dictionary<string, dynamic> {
                    //    {
                    //        "token", token
                    //    },
                    //    {
                    //        "request", new Dictionary<string, string> {
                    //            { "sp_key", SP_Key},
                    //            { "agentid", agentId},
                    //            {"customer_mobile", CUS_Mobile},
                    //            {"customer_mobile", CUS_Mobile},
                    //            {"customer_params", "['','{{}}']"},
                    //            { "init_channel","AGT"},
                    //            {"endpoint_ip","115.96.143.199" },
                    //            {"mac"," 00-0F-22-00-ED-D2" },
                    //            {"payment_mode","Cash" },
                    //            {"payment_info","Bill" },
                    //            {"amount",Amount },
                    //            {"reference_id","" },
                    //            {"latlong","" },
                    //            {"outletid","6601" },
                    //        }

                    //    }
                    //};
                    //var res = GetResponse(url, "POST", param);
                    var res = GetResponse(url, "POST", temp);
                    string errorcode = res.statuscode;//res.res_code;
                    string status = res.status;
                    if (errorcode == "TXN" || errorcode == "TUP" || errorcode == "ERR")
                    {
                        //APIMsg = errorcode;
                        string ipay_id = res.data.ipay_id;
                        //APIMsg = errorcode;
                        return res;
                    }
                    else
                    {
                        //APIMsg = InstantPayError.GetError(res.ipay_errorcode.Value);
                        return res;
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }

            }

            public static dynamic BBPSBillPaymentGASBILL(string SP_Key, string CUS_Mobile, string Cus_Param, string Amount, string latlong, string Outlet, string Ref_ID)
            {
                try
                {
                    List<string> objvalcus = new List<string>();
                    objvalcus.Add(CUS_Mobile);
                    objvalcus.Add(Cus_Param);
                    dynamic LandlineObj = new JObject();
                    LandlineObj.token = token;
                    dynamic RequestVal = new JObject();
                    RequestVal.sp_key = SP_Key;
                    RequestVal.agentid = agentId;
                    RequestVal.customer_mobile = CUS_Mobile;
                    RequestVal.customer_params = new JArray(CUS_Mobile, Cus_Param);
                    RequestVal.init_channel = "AGT";
                    RequestVal.endpoint_ip = EdnPointIP;
                    RequestVal.mac = MACIP;
                    RequestVal.payment_mode = "Cash";
                    RequestVal.payment_info = "bill";
                    RequestVal.amount = Amount;
                    RequestVal.reference_id = Ref_ID;
                    //RequestVal.latlong = latlong.Replace("\"", "");
                    RequestVal.latlong = "22.5844,88.3311";
                    RequestVal.outletid = Outlet;
                    LandlineObj.request = new JObject(RequestVal);

                    string SearchparamValue = JsonConvert.SerializeObject(LandlineObj);
                    Dictionary<string, dynamic> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(SearchparamValue);
                    //var jToken = JToken.Parse(LandlineObj);
                    //var users = jToken.ToObject<List<ParameterBBPS>>();
                    //Dictionary<string, string> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(LandlineObj);
                    //var wordDictionary = new Dictionary<string, List<string>>();
                    //wordDictionary.Add(CUS_Mobile);

                    string url = root + "/ws/bbps/bill_pay";
                    //    var param = new Dictionary<string, dynamic> {
                    //    {
                    //        "token", token
                    //    },
                    //    {
                    //        "request", new Dictionary<string, string> {
                    //            { "sp_key", SP_Key},
                    //            { "agentid", agentId},
                    //            {"customer_mobile", CUS_Mobile},
                    //            {"customer_mobile", CUS_Mobile},
                    //            {"customer_params", "['','{{}}']"},
                    //            { "init_channel","AGT"},
                    //            {"endpoint_ip","115.96.143.199" },
                    //            {"mac"," 00-0F-22-00-ED-D2" },
                    //            {"payment_mode","Cash" },
                    //            {"payment_info","Bill" },
                    //            {"amount",Amount },
                    //            {"reference_id","" },
                    //            {"latlong","" },
                    //            {"outletid","6601" },
                    //        }

                    //    }
                    //};
                    //var res = GetResponse(url, "POST", param);
                    var res = GetResponse(url, "POST", temp);
                    string errorcode = res.statuscode;//res.res_code;
                    string status = res.status;
                    if (errorcode == "TXN" || errorcode == "TUP" || errorcode == "ERR")
                    {
                        //APIMsg = errorcode;
                        string ipay_id = res.data.ipay_id;
                        //APIMsg = errorcode;
                        return res;
                    }
                    else
                    {
                        //APIMsg = InstantPayError.GetError(res.ipay_errorcode.Value);
                        return res;
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }

            }

            public static dynamic BBPSBillPaymentWATER(string SP_Key, string CUS_Mobile, string Cus_Param, string Amount, string latlong, string Outlet, string Ref_ID)
            {
                try
                {
                    List<string> objvalcus = new List<string>();
                    objvalcus.Add(CUS_Mobile);
                    objvalcus.Add(Cus_Param);
                    dynamic LandlineObj = new JObject();
                    LandlineObj.token = token;
                    dynamic RequestVal = new JObject();
                    RequestVal.sp_key = SP_Key;
                    RequestVal.agentid = agentId;
                    RequestVal.customer_mobile = CUS_Mobile;
                    RequestVal.customer_params = new JArray(CUS_Mobile, Cus_Param);
                    RequestVal.init_channel = "AGT";
                    RequestVal.endpoint_ip = EdnPointIP;
                    RequestVal.mac = MACIP;
                    RequestVal.payment_mode = "Cash";
                    RequestVal.payment_info = "bill";
                    RequestVal.amount = Amount;
                    RequestVal.reference_id = Ref_ID;
                    //RequestVal.latlong = latlong.Replace("\"", "");
                    RequestVal.latlong = "22.5844,88.3311";
                    RequestVal.outletid = Outlet;
                    LandlineObj.request = new JObject(RequestVal);

                    string SearchparamValue = JsonConvert.SerializeObject(LandlineObj);
                    Dictionary<string, dynamic> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(SearchparamValue);
                    //var jToken = JToken.Parse(LandlineObj);
                    //var users = jToken.ToObject<List<ParameterBBPS>>();
                    //Dictionary<string, string> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(LandlineObj);
                    //var wordDictionary = new Dictionary<string, List<string>>();
                    //wordDictionary.Add(CUS_Mobile);

                    string url = root + "/ws/bbps/bill_pay";
                    //    var param = new Dictionary<string, dynamic> {
                    //    {
                    //        "token", token
                    //    },
                    //    {
                    //        "request", new Dictionary<string, string> {
                    //            { "sp_key", SP_Key},
                    //            { "agentid", agentId},
                    //            {"customer_mobile", CUS_Mobile},
                    //            {"customer_mobile", CUS_Mobile},
                    //            {"customer_params", "['','{{}}']"},
                    //            { "init_channel","AGT"},
                    //            {"endpoint_ip","115.96.143.199" },
                    //            {"mac"," 00-0F-22-00-ED-D2" },
                    //            {"payment_mode","Cash" },
                    //            {"payment_info","Bill" },
                    //            {"amount",Amount },
                    //            {"reference_id","" },
                    //            {"latlong","" },
                    //            {"outletid","6601" },
                    //        }

                    //    }
                    //};
                    //var res = GetResponse(url, "POST", param);
                    var res = GetResponse(url, "POST", temp);
                    string errorcode = res.statuscode;//res.res_code;
                    string status = res.status;
                    if (errorcode == "TXN" || errorcode == "TUP" || errorcode == "ERR")
                    {
                        //APIMsg = errorcode;
                        string ipay_id = res.data.ipay_id;
                        //APIMsg = errorcode;
                        return res;
                    }
                    else
                    {
                        //APIMsg = InstantPayError.GetError(res.ipay_errorcode.Value);
                        return res;
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }

            }


            public static dynamic BBPSBillPayment(string SP_Key, string CUS_Mobile, string Cus_Param, string Amount, string latlong, string Outlet,string Agent_Id)
            {
                try
                {
                    List<string> objvalcus = new List<string>();
                    objvalcus.Add(CUS_Mobile);
                    objvalcus.Add(Cus_Param);
                    dynamic LandlineObj = new JObject();
                    LandlineObj.token = token;
                    dynamic RequestVal = new JObject();
                    RequestVal.sp_key = SP_Key;
                    RequestVal.agentid = Agent_Id;
                    RequestVal.customer_mobile = CUS_Mobile;
                    RequestVal.customer_params = new JArray(Cus_Param, CUS_Mobile);
                    RequestVal.init_channel = "AGT";
                    RequestVal.endpoint_ip = EdnPointIP;
                    RequestVal.mac = MACIP;
                    RequestVal.payment_mode = "Cash";
                    RequestVal.payment_info = "bill";
                    RequestVal.amount = Amount;
                    RequestVal.reference_id = "";
                    //RequestVal.latlong = latlong.Replace("\"", "");
                    RequestVal.latlong = "22.5844,88.3311";
                    RequestVal.outletid = Outlet;
                    LandlineObj.request = new JObject(RequestVal);

                    string SearchparamValue = JsonConvert.SerializeObject(LandlineObj);
                    Dictionary<string, dynamic> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(SearchparamValue);
                    //var jToken = JToken.Parse(LandlineObj);
                    //var users = jToken.ToObject<List<ParameterBBPS>>();
                    //Dictionary<string, string> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(LandlineObj);
                    //var wordDictionary = new Dictionary<string, List<string>>();
                    //wordDictionary.Add(CUS_Mobile);

                    string url = root + "/ws/bbps/bill_pay";
                    //    var param = new Dictionary<string, dynamic> {
                    //    {
                    //        "token", token
                    //    },
                    //    {
                    //        "request", new Dictionary<string, string> {
                    //            { "sp_key", SP_Key},
                    //            { "agentid", agentId},
                    //            {"customer_mobile", CUS_Mobile},
                    //            {"customer_mobile", CUS_Mobile},
                    //            {"customer_params", "['','{{}}']"},
                    //            { "init_channel","AGT"},
                    //            {"endpoint_ip","115.96.143.199" },
                    //            {"mac"," 00-0F-22-00-ED-D2" },
                    //            {"payment_mode","Cash" },
                    //            {"payment_info","Bill" },
                    //            {"amount",Amount },
                    //            {"reference_id","" },
                    //            {"latlong","" },
                    //            {"outletid","6601" },
                    //        }

                    //    }
                    //};
                    //var res = GetResponse(url, "POST", param);
                    var res = GetResponse(url, "POST", temp);
                    string errorcode = res.statuscode;//res.res_code;
                    string status = res.status;
                    if (errorcode == "TXN" || errorcode == "TUP" || errorcode == "ERR")
                    {
                        //APIMsg = errorcode;
                        string ipay_id = res.data.ipay_id;
                        //APIMsg = errorcode;
                        return res;
                    }
                    else
                    {
                        //APIMsg = InstantPayError.GetError(res.ipay_errorcode.Value);
                        return res;
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }

            }


            public static dynamic BBPSBillPaymentElectricityValidation(string SP_Key, string CUS_Mobile,string BillUnit ,string Cus_Param, string Amount, string latlong, string Outlet,string CityName,string Agent_ID)
            {
                try
                {
                    string transactionstatus = string.Empty;
                    List<string> objvalcus = new List<string>();
                    objvalcus.Add(CUS_Mobile);
                    objvalcus.Add(Cus_Param);
                    dynamic LandlineObj = new JObject();
                    LandlineObj.token = token;
                    dynamic RequestVal = new JObject();
                    RequestVal.sp_key = SP_Key;
                    RequestVal.agentid = Agent_ID;
                    RequestVal.customer_mobile = CUS_Mobile;
                    RequestVal.customer_params = new JArray(Cus_Param,BillUnit, CityName);
                    RequestVal.init_channel = "AGT";
                    RequestVal.endpoint_ip = EdnPointIP;
                    RequestVal.mac = MACIP;
                    RequestVal.payment_mode = "Cash";
                    RequestVal.payment_info = "bill";
                    //RequestVal.payment_info = "11";
                    RequestVal.amount = Amount;
                    RequestVal.reference_id = "";
                    //RequestVal.latlong = latlong.Replace("\"", "");
                    RequestVal.latlong = "22.5844,88.3311";
                    RequestVal.outletid = Outlet;
                    LandlineObj.request = new JObject(RequestVal);

                    string SearchparamValue = JsonConvert.SerializeObject(LandlineObj);
                    Dictionary<string, dynamic> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(SearchparamValue);
                    //var jToken = JToken.Parse(LandlineObj);
                    //var users = jToken.ToObject<List<ParameterBBPS>>();
                    //Dictionary<string, string> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(LandlineObj);
                    //var wordDictionary = new Dictionary<string, List<string>>();
                    //wordDictionary.Add(CUS_Mobile);

                    string url = root + "/ws/bbps/bill_fetch";
                    //    var param = new Dictionary<string, dynamic> {
                    //    {
                    //        "token", token
                    //    },
                    //    {
                    //        "request", new Dictionary<string, string> {
                    //            { "sp_key", SP_Key},
                    //            { "agentid", agentId},
                    //            {"customer_mobile", CUS_Mobile},
                    //            {"customer_mobile", CUS_Mobile},
                    //            {"customer_params", "['','{{}}']"},
                    //            { "init_channel","AGT"},
                    //            {"endpoint_ip","115.96.143.199" },
                    //            {"mac"," 00-0F-22-00-ED-D2" },
                    //            {"payment_mode","Cash" },
                    //            {"payment_info","Bill" },
                    //            {"amount",Amount },
                    //            {"reference_id","" },
                    //            {"latlong","" },
                    //            {"outletid","6601" },
                    //        }

                    //    }
                    //};
                    //var res = GetResponse(url, "POST", param);
                    var res = GetResponse(url, "POST", temp);
                    string errorcode = res.statuscode;
                    string des = res.status;
                    if (errorcode == "TXN")
                    {
                        return res;
                    }
                    else
                    {
                        //return InstantPayError.GetError(res.statuscode.Value);
                        //transactionstatus = InstantPayError.GetError(errorcode); ;
                        return res;
                        //transactionstatus = des;
                    }
                    //return transactionstatus;
                }
                catch (Exception ex)
                {
                    var msg = "Unauthorized Access";
                    return msg;
                    throw;
                }

            }

            public static dynamic BBPSBillPaymentELECTRICITY(string SP_Key, string CUS_Mobile,string BillUnit ,string Cus_Param, string Amount, string latlong, string Outlet,string Reference_ID,string CityName,string Agent_ID)
            {
                try
                {
                    List<string> objvalcus = new List<string>();
                    objvalcus.Add(CUS_Mobile);
                    objvalcus.Add(Cus_Param);
                    dynamic LandlineObj = new JObject();
                    LandlineObj.token = token;
                    dynamic RequestVal = new JObject();
                    RequestVal.sp_key = SP_Key;
                    RequestVal.agentid = agentId;
                    RequestVal.customer_mobile = CUS_Mobile;
                    //RequestVal.customer_params = new JArray(Cus_Param, BillUnit, CityName);
                    RequestVal.customer_params = new JArray(Cus_Param, BillUnit);
                    RequestVal.init_channel = "AGT";
                    RequestVal.endpoint_ip = EdnPointIP;
                    RequestVal.mac = MACIP;
                    RequestVal.payment_mode = "cash";
                    RequestVal.payment_info = "bill";
                    //RequestVal.payment_info = "11";
                    RequestVal.amount = Amount;
                    RequestVal.reference_id = Reference_ID;
                    //RequestVal.latlong = latlong.Replace("\"", "");
                    RequestVal.latlong = "22.5844,88.3311";
                    RequestVal.outletid = Outlet;
                    LandlineObj.request = new JObject(RequestVal);

                    string SearchparamValue = JsonConvert.SerializeObject(LandlineObj);
                    Dictionary<string, dynamic> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(SearchparamValue);
                    //var jToken = JToken.Parse(LandlineObj);
                    //var users = jToken.ToObject<List<ParameterBBPS>>();
                    //Dictionary<string, string> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(LandlineObj);
                    //var wordDictionary = new Dictionary<string, List<string>>();
                    //wordDictionary.Add(CUS_Mobile);

                    string url = root + "/ws/bbps/bill_pay";
                    //    var param = new Dictionary<string, dynamic> {
                    //    {
                    //        "token", token
                    //    },
                    //    {
                    //        "request", new Dictionary<string, string> {
                    //            { "sp_key", SP_Key},
                    //            { "agentid", agentId},
                    //            {"customer_mobile", CUS_Mobile},
                    //            {"customer_mobile", CUS_Mobile},
                    //            {"customer_params", "['','{{}}']"},
                    //            { "init_channel","AGT"},
                    //            {"endpoint_ip","115.96.143.199" },
                    //            {"mac"," 00-0F-22-00-ED-D2" },
                    //            {"payment_mode","Cash" },
                    //            {"payment_info","Bill" },
                    //            {"amount",Amount },
                    //            {"reference_id","" },
                    //            {"latlong","" },
                    //            {"outletid","6601" },
                    //        }

                    //    }
                    //};
                    //var res = GetResponse(url, "POST", param);
                    var res = GetResponse(url, "POST", temp);
                    string errorcode = res.statuscode;//res.res_code;
                    string status = res.status;
                    if (errorcode == "TXN" || errorcode == "TUP" || errorcode == "ERR")
                    {
                        //APIMsg = errorcode;
                        string ipay_id = res.data.ipay_id;
                        //APIMsg = errorcode;
                        return res;
                    }
                    else
                    {
                        //APIMsg = InstantPayError.GetError(res.ipay_errorcode.Value);
                        return res;
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }

            }

            public static dynamic BBPSBillPaymentWaterBillValidation(string SP_Key, string CUS_Mobile, string BillUnit, string Cus_Param, string Amount, string latlong, string Outlet,string Agent_ID)
            {
                try
                {
                    string transactionstatus = string.Empty;
                    List<string> objvalcus = new List<string>();
                    objvalcus.Add(CUS_Mobile);
                    objvalcus.Add(Cus_Param);
                    dynamic LandlineObj = new JObject();
                    LandlineObj.token = token;
                    dynamic RequestVal = new JObject();
                    RequestVal.sp_key = SP_Key;
                    RequestVal.agentid = Agent_ID;
                    RequestVal.customer_mobile = CUS_Mobile;
                    RequestVal.customer_params = new JArray(Cus_Param, BillUnit);
                    RequestVal.init_channel = "AGT";
                    RequestVal.endpoint_ip = EdnPointIP;
                    RequestVal.mac = MACIP;
                    RequestVal.payment_mode = "Cash";
                    RequestVal.payment_info = "BILL";
                    RequestVal.amount = Amount;
                    RequestVal.reference_id = "";
                    //RequestVal.latlong = latlong.Replace("\"", "");
                    RequestVal.latlong = "22.5844,88.3311";
                    RequestVal.outletid = Outlet;
                    LandlineObj.request = new JObject(RequestVal);

                    string SearchparamValue = JsonConvert.SerializeObject(LandlineObj);
                    Dictionary<string, dynamic> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(SearchparamValue);
                    //var jToken = JToken.Parse(LandlineObj);
                    //var users = jToken.ToObject<List<ParameterBBPS>>();
                    //Dictionary<string, string> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(LandlineObj);
                    //var wordDictionary = new Dictionary<string, List<string>>();
                    //wordDictionary.Add(CUS_Mobile);

                    string url = root + "/ws/bbps/bill_fetch";
                    //    var param = new Dictionary<string, dynamic> {
                    //    {
                    //        "token", token
                    //    },
                    //    {
                    //        "request", new Dictionary<string, string> {
                    //            { "sp_key", SP_Key},
                    //            { "agentid", agentId},
                    //            {"customer_mobile", CUS_Mobile},
                    //            {"customer_mobile", CUS_Mobile},
                    //            {"customer_params", "['','{{}}']"},
                    //            { "init_channel","AGT"},
                    //            {"endpoint_ip","115.96.143.199" },
                    //            {"mac"," 00-0F-22-00-ED-D2" },
                    //            {"payment_mode","Cash" },
                    //            {"payment_info","Bill" },
                    //            {"amount",Amount },
                    //            {"reference_id","" },
                    //            {"latlong","" },
                    //            {"outletid","6601" },
                    //        }

                    //    }
                    //};
                    //var res = GetResponse(url, "POST", param);
                    var res = GetResponse(url, "POST", temp);
                    string errorcode = res.statuscode;
                    string des = res.status;
                    if (errorcode == "TXN")
                    {
                        return res;
                    }
                    else
                    {
                        //return InstantPayError.GetError(res.statuscode.Value);
                        //transactionstatus = InstantPayError.GetError(errorcode); ;
                        return res;
                        //transactionstatus = des;
                    }
                    //return transactionstatus;
                }
                catch (Exception ex)
                {

                    throw;
                }

            }

            public static dynamic BBPSBillPaymentWaterBill(string SP_Key, string CUS_Mobile, string BillUnit, string Cus_Param, string Amount, string latlong, string Outlet, string Reference_ID,string Agent_ID)
            {
                try
                {
                    List<string> objvalcus = new List<string>();
                    objvalcus.Add(CUS_Mobile);
                    objvalcus.Add(Cus_Param);
                    dynamic LandlineObj = new JObject();
                    LandlineObj.token = token;
                    dynamic RequestVal = new JObject();
                    RequestVal.sp_key = SP_Key;
                    RequestVal.agentid = Agent_ID;
                    RequestVal.customer_mobile = CUS_Mobile;
                    RequestVal.customer_params = new JArray(Cus_Param, BillUnit);
                    RequestVal.init_channel = "AGT";
                    RequestVal.endpoint_ip = EdnPointIP;
                    RequestVal.mac = MACIP;
                    RequestVal.payment_mode = "Cash";
                    RequestVal.payment_info = "bill";
                    RequestVal.amount = Amount;
                    RequestVal.reference_id = Reference_ID;
                    //RequestVal.latlong = latlong.Replace("\"", "");
                    RequestVal.latlong = "22.5844,88.3311";
                    RequestVal.outletid = Outlet;
                    LandlineObj.request = new JObject(RequestVal);

                    string SearchparamValue = JsonConvert.SerializeObject(LandlineObj);
                    Dictionary<string, dynamic> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(SearchparamValue);
                    //var jToken = JToken.Parse(LandlineObj);
                    //var users = jToken.ToObject<List<ParameterBBPS>>();
                    //Dictionary<string, string> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(LandlineObj);
                    //var wordDictionary = new Dictionary<string, List<string>>();
                    //wordDictionary.Add(CUS_Mobile);

                    string url = root + "/ws/bbps/bill_pay";
                    //    var param = new Dictionary<string, dynamic> {
                    //    {
                    //        "token", token
                    //    },
                    //    {
                    //        "request", new Dictionary<string, string> {
                    //            { "sp_key", SP_Key},
                    //            { "agentid", agentId},
                    //            {"customer_mobile", CUS_Mobile},
                    //            {"customer_mobile", CUS_Mobile},
                    //            {"customer_params", "['','{{}}']"},
                    //            { "init_channel","AGT"},
                    //            {"endpoint_ip","115.96.143.199" },
                    //            {"mac"," 00-0F-22-00-ED-D2" },
                    //            {"payment_mode","Cash" },
                    //            {"payment_info","Bill" },
                    //            {"amount",Amount },
                    //            {"reference_id","" },
                    //            {"latlong","" },
                    //            {"outletid","6601" },
                    //        }

                    //    }
                    //};
                    //var res = GetResponse(url, "POST", param);
                    var res = GetResponse(url, "POST", temp);
                    string errorcode = res.statuscode;//res.res_code;
                    string status = res.status;
                    if (errorcode == "TXN" || errorcode == "TUP" || errorcode == "ERR")
                    {
                        //APIMsg = errorcode;
                        string ipay_id = res.data.ipay_id;
                        //APIMsg = errorcode;
                        return res;
                    }
                    else
                    {
                        //APIMsg = InstantPayError.GetError(res.ipay_errorcode.Value);
                        return res;
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }

            }


        }

        public static class ElectricityPaymentAPI
        {
            public static string ElectricityValidation(string agentid, string amount, string Mobile, string spkey, string account, string pin, string outletid,string Agent_ID)
            {
                try
                {
                    string transactionstatus = string.Empty;
                    var listinfo = "";
                    string url = root + "/ws/api/transaction?format=json&token=" + token + "&agentid=" + Agent_ID + "&amount=" + amount + "&spkey=" + spkey + "&account=" + account + "&mode=VALIDATE&optional1={{optional1}}&optional2={{optional2}}&optional3={{optional3}}&optional4={{optional4}}&optional5={{optional5}}&optional6={{optional6}}&optional7={{optional7}}&optional8=remarks&optional9=" + pin + "&outletid=" + outletid + "&endpointip={{customer_ip}}&customermobile=" + Mobile + "&paymentmode=CASH&paymentchannel=AGT";
                    var res = GetResponse(url, "GET", new Dictionary<string, string>());
                    string errorcode = res.ipay_errorcode;
                    string des = res.ipay_errordesc;
                    if (errorcode == "TXN")
                    {
                        transactionstatus = errorcode;
                    }
                    else
                    {
                        transactionstatus = InstantPayError.GetError(errorcode);
                    }
                    return transactionstatus;
                    //if(res.)
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            public static dynamic ElectricityPayment(string agentid, string amount, string Mobile, string spkey, string account, string pin, string outletid,string Agent_ID)
            {
                try
                {
                    string APIMsg = string.Empty;
                    string url = root + "/ws/api/transaction?format=json&token=" + token + "&spkey=" + spkey + "&agentid=" + Agent_ID + "&amount=" + amount + "&account=" + account + "&optional1={{optional1}}&optional2={{optional2}}&optional3={{optional3}}&optional4={{optional4}}&optional5={{optional5}}&optional6={{optional6}}&optional7={{optional7}}&optional8={{optional8}}&optional9=" + pin + "&outletid=" + outletid + "&endpointip={{customer_ip}}&customermobile=" + Mobile + "&paymentmode={{paymentmode}}&paymentchannel={{paymentchannel}}";
                    var res = GetResponse(url, "GET", new Dictionary<string, string>());
                    string errorcode = res.res_code;//res.res_code;
                    string status = res.status;
                    if (errorcode == "TXN")
                    {
                        //APIMsg = errorcode;
                        string ipay_id = res.ipay_id;
                        //APIMsg = errorcode;
                        return res;
                    }
                    else
                    {
                        //APIMsg = InstantPayError.GetError(res.ipay_errorcode.Value);
                        return res;
                    }
                    //return APIMsg;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            public static dynamic ElectricityStatusCheck(string agentid)
            {
                try
                {
                    string APIMsg = string.Empty;
                    string url = root + "/ws/api/getMIS?format=json&token=" + token + "&agentid=" + agentid;
                    var res = GetResponse(url, "GET", new Dictionary<string, string>());
                    string errorcode = res.res_code;
                    if (errorcode == "TXN")
                    {
                        //APIMsg = errorcode;
                        return res;
                    }
                    else
                    {
                        //APIMsg = InstantPayError.GetError(errorcode);
                        return res;
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            public static dynamic ElectricityServiceProviderDetails(string spkey)
            {
                try
                {
                    string url = root + "/ws/api/serviceproviders?token=" + token + "&spkey=" + spkey + "&format=json";
                    var res = GetResponse(url, "GET", new Dictionary<string, string>());
                    string errorcode = res.ipay_errorcode;
                    if (errorcode == "TXN")
                    {
                        return res;
                    }
                    else
                    {
                        return InstantPayError.GetError(errorcode);
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public static class MoneyTransferAPI
        {
            public static dynamic RemitterDetails(string mobile)
            {
                string url = root + "/ws/dmi/remitter_details";
                var param = new Dictionary<string, dynamic> {
                    {
                        "token", token
                    },
                    {
                        "request", new Dictionary<string, string> {
                            { "mobile", mobile}
                        }

                    }
                };
                var res = GetResponse(url, "POST", param);
                if (res.statuscode.Value == "TXN")
                {
                    return res;
                }
                else
                {
                    //return InstantPayError.GetError(res.statuscode.Value);
                    return res;
                }
            }
            public static string RemitterID = string.Empty;
            public static dynamic RemitterRegistration(string mobile, string name, string pincode)
            {
                string url = root + "/ws/dmi/remitter";
                var param = new Dictionary<string, dynamic> {
                    {
                        "token", token
                    },
                    {
                        "request", new Dictionary<string, string> {
                            { "mobile" , mobile },
                            { "name" , name },
                            { "pincode" , pincode }
                        }
                    }
                };
                var res = GetResponse(url, "POST", param);
                if (res.statuscode.Value == "TXN")
                {
                    RemitterID = res.data.remitter.id;
                    return res;
                }
                else
                {
                    //return InstantPayError.GetError(res.statuscode.Value);
                    return res;
                }
            }
            public static dynamic BeneficiaryRegistration(string remitterid, string mobile, string name, string ifsc, string account)
            {
                string url = root + "/ws/dmi/beneficiary_register";
                var param = new Dictionary<string, dynamic> {
                    {
                        "token", token
                    },
                    {
                        "request", new Dictionary<string, string> {
                            { "remitterid",remitterid},
                            { "mobile" , mobile },
                            { "name" , name },
                            {"ifsc",ifsc },
                            { "account",account}
                        }
                    }
                };
                var res = GetResponse(url, "POST", param);
                if (res.statuscode.Value == "TXN")
                {
                    return res;
                }
                else
                {
                    //return InstantPayError.GetError(res.statuscode.Value);
                    return res;
                }
            }
            public static dynamic BeneficiaryRegistrationResendOTP(string remitterid, string beneficiaryid)
            {
                try
                {
                    string url = root + "/ws/dmi/beneficiary_resend_otp";
                    var param = new Dictionary<string, dynamic> {
                    {
                        "token", token
                    },
                    {
                        "request", new Dictionary<string, string> {
                             { "remitterid",remitterid},
                             { "beneficiaryid",beneficiaryid}
                        }
                    }
                };
                    var res = GetResponse(url, "POST", param);
                    if (res.statuscode.Value == "TXN")
                    {
                        return res;
                    }
                    else
                    {
                        //return InstantPayError.GetError(res.statuscode.Value);
                        return res;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            public static dynamic BeneficiaryRegistrationValidate(string remitterid, string beneficiaryid, string otp)
            {
                try
                {
                    string url = root + "/ws/dmi/beneficiary_register_validate";
                    var param = new Dictionary<string, dynamic> {
                    {
                        "token", token
                    },
                    {
                        "request", new Dictionary<string, string> {
                             { "remitterid",remitterid},
                             { "beneficiaryid",beneficiaryid},
                             {"otp", otp}
                        }
                    }
                };
                    var res = GetResponse(url, "POST", param);
                    if (res.statuscode.Value == "TXN")
                    {
                        return res;
                    }
                    else
                    {
                        //return InstantPayError.GetError(res.statuscode.Value);
                        return res;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            public static dynamic BeneficiaryAccountVerification(string remittermobile, string account, string ifsc, string agentid)
            {
                try
                {
                    string url = root + "/ws/imps/account_validate";
                    var param = new Dictionary<string, dynamic> {
                    {
                        "token", token
                    },
                    {
                        "request", new Dictionary<string, string> {
                             { "remittermobile",remittermobile},
                             { "account",account},
                             {"ifsc", ifsc},
                             {"agentid",agentid }
                        }
                    }
                };
                    var res = GetResponse(url, "POST", param);
                    if (res.statuscode.Value == "TXN")
                    {
                        return res;
                    }
                    else
                    {
                        //return InstantPayError.GetError(res.statuscode.Value);
                        return res;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            public static dynamic BeneficiaryDelete(string remitterid, string beneficiaryid)
            {
                try
                {
                    string url = root + "/ws/dmi/beneficiary_remove";
                    var param = new Dictionary<string, dynamic> {
                    {
                        "token", token
                    },
                    {
                        "request", new Dictionary<string, string> {
                            { "beneficiaryid",beneficiaryid},
                            { "remitterid",remitterid}

                        }
                    }
                };
                    var res = GetResponse(url, "POST", param);
                    if (res.statuscode.Value == "TXN")
                    {
                        return res;
                    }
                    else
                    {
                        //return InstantPayError.GetError(res.statuscode.Value);
                        return res;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            public static dynamic BeneficiaryDeleteValidate(string remitterid, string beneficiaryid, string otp)
            {
                try
                {
                    string url = root + "/ws/dmi/beneficiary_remove_validate";
                    var param = new Dictionary<string, dynamic> {
                    {
                        "token", token
                    },
                    {
                        "request", new Dictionary<string, string> {
                            { "beneficiaryid",beneficiaryid},
                            { "remitterid",remitterid},
                            {"otp",otp }

                        }
                    }
                };
                    var res = GetResponse(url, "POST", param);
                    if (res.statuscode.Value == "TXN")
                    {
                        return res;
                    }
                    else
                    {
                        //return InstantPayError.GetError(res.statuscode.Value);
                        return res;
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            public static dynamic FundTransfer(string remittermobile, string beneficiaryid, string agentid, string amount, string mode)
            {
                try
                {
                    string url = root + "/ws/dmi/transfer";
                    var param = new Dictionary<string, dynamic> {
                    {
                        "token", token
                    },
                    {
                        "request", new Dictionary<string, string> {
                            { "remittermobile",remittermobile},
                            { "beneficiaryid",beneficiaryid},
                            { "agentid",agentid},
                            { "amount",amount},
                            {"mode",mode }

                        }
                    }
                };
                    var res = GetResponse(url, "POST", param);
                    if (res.statuscode.Value == "TXN")
                    {
                        return res;
                    }
                    else
                    {
                        //return InstantPayError.GetError(res.statuscode.Value);
                        return res;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            public static dynamic FundTransferStatus(string ipayid)
            {
                try
                {
                    string url = root + "/ws/dmi/transfer_status";
                    var param = new Dictionary<string, dynamic> {
                    {
                        "token", token
                    },
                    {
                        "request", new Dictionary<string, string> {
                            { "ipayid",ipayid}

                        }
                    }
                };
                    var res = GetResponse(url, "POST", param);
                    if (res.statuscode.Value == "TXN")
                    {
                        return res;
                    }
                    else
                    {
                        //return InstantPayError.GetError(res.statuscode.Value);
                        return res;
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            public static dynamic GetBankDetails(string account)
            {
                try
                {
                    string url = root + "/ws/dmi/bank_details";
                    var param = new Dictionary<string, dynamic> {
                    {
                        "token", token
                    },
                    {
                        "request", new Dictionary<string, string> {
                            { "account",account}

                        }
                    }
                };
                    var res = GetResponse(url, "POST", param);
                    if (res.statuscode.Value == "TXN")
                    {
                        return res;
                    }
                    else
                    {
                        return InstantPayError.GetError(res.statuscode.Value);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            public static dynamic RemitterKYC(string Mobile, string Aadhaar)
            {
                try
                {
                    string url = root + "/ws/kyc/remitter";
                    var param = new Dictionary<string, dynamic> {
                    {
                        "token", token
                    },
                    {
                        "request", new Dictionary<string, string> {
                            { "mobile_number",Mobile},
                            { "aadhaar_number",Aadhaar}

                        }
                    }
                };
                    var res = GetResponse(url, "POST", param);
                    if (res.statuscode.Value == "TXN")
                    {
                        return res;
                    }
                    else
                    {
                        return InstantPayError.GetError(res.statuscode.Value);
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }

            public static dynamic RemitterKYCValidate(string otp, string mobile_number, string aadhaar_number, string aadhaar_card, string filename)
            {
                try
                {
                    string url = root + "/ws/kyc/remitter/validate";
                    var param = new Dictionary<string, dynamic> {
                    {
                        "token", token
                    },
                    {
                        "request", new Dictionary<string, string> {
                            { "otp",otp},
                            { "mobile_number",mobile_number},
                            { "aadhaar_number",aadhaar_number},
                            { "aadhaar_card",aadhaar_card},
                            { "filename",filename}
                        }
                    }
                };
                    var res = GetResponse(url, "POST", param);
                    if (res.statuscode.Value == "TXN")
                    {
                        return res;
                    }
                    else
                    {
                        return InstantPayError.GetError(res.statuscode.Value);
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            public static dynamic RemitterKYCV2(dynamic Request)
            {

                //string Url = SIMTIKFRONT.Web.Domain.AppSettings.DwollaURL + "/customers/" + DwollaId + "/documents";
                string Url = root + "/ws/kyc/remitter/validate_v2";
                HttpContent fileStreamContent = new StreamContent(Request.InputStream);
                using (HttpClient client = ClientHelper.GetDocClient())
                {
                    using (var formData = new MultipartFormDataContent())
                    {
                        Uri uploadUrlUri = new Uri(Url);

                        //formData.Add(new StringContent("idCard"), "documentType");

                        formData.Add(fileStreamContent, "token", token);
                        formData.Add(fileStreamContent, "aadhaar_card", "");
                        formData.Add(fileStreamContent, "otp", "");
                        formData.Add(fileStreamContent, "mobile_number", "9903116214");
                        formData.Add(fileStreamContent, "aadhaar_number", "9903116214");

                        var response = client.PostAsync(uploadUrlUri, formData).Result;

                        return Regex.Split(response.Headers.Location.ToString(), "/")[4];

                    }
                }


            }


        }

        private static dynamic GetResponse<T>(string url, string method, Dictionary<string, T> param)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                using (HttpClient client = ClientHelper.GetClient())
                {
                    switch (method.ToUpper())
                    {
                        case "GET":
                            {
                                response = client.GetAsync(url).Result;
                                break;
                            }
                        case "POST":
                            {
                                response = client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json")).Result;
                                break;
                            }
                        default:
                            {
                                throw new NotImplementedException();
                            }
                    }
                    response.EnsureSuccessStatusCode();
                    return string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result) ? response : JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception e)
            {
                // Handle exception
                throw e;
            }
        }
        public static class ClientHelper
        {
            public static HttpClient GetClient()
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                return client;
            }
            public static HttpClient GetDocClient()
            {
                HttpClient Docclient = new HttpClient();
                Docclient.DefaultRequestHeaders.Accept.Clear();
                Docclient.DefaultRequestHeaders.Add("Accept", "application/json");
                Docclient.DefaultRequestHeaders.Add("Content-Type", "application/json");
                Docclient.DefaultRequestHeaders.Add("ContentType", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");
                return Docclient;
            }
        }

        public static class OutletApi
        {
            public static dynamic VerifyOutlet(string mobile)
            {
                //string url = root + "/ws/outlet/sendOTP";
                string url = root + "/ws/outlet/registrationOTP";/*registrationOTP*/
                var param = new Dictionary<string, dynamic> {
                    {
                        "token", token
                    },
                    {
                        "request", new Dictionary<string, string> {
                            { "mobile", mobile}
                        }

                    }
                };
                var res = GetResponse(url, "POST", param);
                if (res.statuscode.Value == "TXN")
                {
                    return res;
                }
                else
                {
                    //return InstantPayError.GetError(res.statuscode.Value);
                    return res;
                }
            }
            public static dynamic RegisterOutlet(string mobile, string otp, string email, string company, string name, string address, string pincode,string PanNo)
            {
                /*string url = root + "/ws/outlet/register";*/ /*registration*/
                string url = root + "/ws/outlet/registration";
                var param = new Dictionary<string, dynamic> {
                    {
                        "token", token
                    },
                    {
                        "request", new Dictionary<string, string> {
                            { "mobile", mobile},                            
                            { "email" , email },
                            {"company",company },
                            { "name",name},
                            {"pan",PanNo },
                            { "pincode",pincode  },
                            { "address",address},                            
                            { "otp" , otp},

                        }

                    }
                };
                var res = GetResponse(url, "POST", param);
                if (res.statuscode.Value == "TXN")
                {
                    return res;
                }
                else
                {
                    //return InstantPayError.GetError(res.statuscode.Value);
                    return res;
                }
            }
        }
        public class Request
        {
            public string sp_key { get; set; }
            public string agentid { get; set; }
            public string customer_mobile { get; set; }
            public List<string> customer_params { get; set; }
            public string init_channel { get; set; }
            public string endpoint_ip { get; set; }
            public string mac { get; set; }
            public string payment_mode { get; set; }
            public string payment_info { get; set; }
            public string amount { get; set; }
            public string latlong { get; set; }
            public string outletid { get; set; }
        }

        public class ParameterBBPS
        {
            public string token { get; set; }
            public Request request { get; set; }
        }


    }
}