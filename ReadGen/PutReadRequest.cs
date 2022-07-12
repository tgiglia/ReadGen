using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Net;
using System.IO;

namespace ReadGen
{
    public class PutReadRequest : RESTBase
    {
        public PutReadRequest(string user, string pass, string url) : base(user, pass, url)
        {
        }
        public HttpStatusCode PutResourceReadRequest(String readGuid, String theXml)
        {
            HttpStatusCode retCode = HttpStatusCode.BadRequest;
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(theXml);
            Console.WriteLine("PutResourceReadRequest: Uri: " + ServerURL);
            Console.WriteLine("PutResourceReadRequest: PUT request to resource /read/" + readGuid);
            //Console.WriteLine("PutResourceReadRequest length: " + theXml.Length);
            // Create a request
            //HttpWebRequest request = WebRequest.Create(ServerURL + "read/" + readGuid) as HttpWebRequest;
            HttpWebRequest request = WebRequest.Create(ServerURL + "read/" + readGuid) as HttpWebRequest;
            request.Method = "PUT";
            request.Credentials = creds;
            if (UsePutCompression == PutRequestCompression.gzip)
                request.Headers.Add(HttpRequestHeader.ContentEncoding, "gzip");


            if (UsePutCompression == PutRequestCompression.deflate)
                request.Headers.Add(HttpRequestHeader.ContentEncoding, "deflate");

            using (Stream requestStream = request.GetRequestStream())
            {
                if (UsePutCompression == PutRequestCompression.none)
                    xml.Save(requestStream);
                else
                {
                    byte[] bytes = Encoding.Default.GetBytes(xml.OuterXml);

                    if (UsePutCompression == PutRequestCompression.gzip)
                    {
                        using (var zipStream = new System.IO.Compression.GZipStream(requestStream, System.IO.Compression.CompressionMode.Compress))
                        {
                            zipStream.Write(bytes, 0, bytes.Length);
                        }
                    }

                    if (UsePutCompression == PutRequestCompression.deflate)
                    {
                        using (var zipStream = new System.IO.Compression.DeflateStream(requestStream, System.IO.Compression.CompressionMode.Compress))
                        {
                            zipStream.Write(bytes, 0, bytes.Length);
                        }
                    }
                }
            }
            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Check the response
                    // WebException may be thrown already for some of this already like timeout or 404
                    retCode = response.StatusCode;
                    if (request.HaveResponse == true && response == null)
                    {
                        //Console.WriteLine("Response was not returned or is null");
                    }
                    else if (response.StatusCode == HttpStatusCode.OK)
                    {
                        //Console.WriteLine("PUT Request sent successfully");
                    }
                    else
                    {
                        //Console.WriteLine("There was an error sending PUT Request");
                    }
                    //Console.WriteLine("Response with status: " + response.StatusCode + " " + response.StatusDescription);

                    string responseContent = null;
                    // Get the response content
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                    //Console.WriteLine("Reponse: " + responseContent);
                }
            }
            catch (WebException WebEx)
            {
                HttpWebResponse ExResponse = (HttpWebResponse)WebEx.Response;

                Console.WriteLine("Error while communicating with server");
                Console.WriteLine(WebEx.Message);
                
                //Logger.logIt(WebEx.Message);
                if (ExResponse != null)
                {
                    Console.WriteLine("Status Code: {0}", (ExResponse).StatusCode);
                    

                    Console.WriteLine("Status Description: {0}", (ExResponse).StatusDescription);

                    using (Stream ExDataStream = ExResponse.GetResponseStream())
                    {
                        using (StreamReader ExReader = new StreamReader(ExDataStream))
                        {
                            string ExResponseFromServer = ExReader.ReadToEnd();

                            Console.WriteLine("HTTP Response Body: {0}", ExResponseFromServer);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return retCode;
        }
        public void PutResourceAlarmRequest(String alarmGuid, String theXml)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(theXml);
            Console.WriteLine("PutResourceReadRequest: Uri: " + ServerURL);
            Console.WriteLine("PutResourceReadRequest: PUT request to resource /alarm/" + alarmGuid);
            Console.WriteLine("PutResourceReadRequest length: " + theXml.Length);
            HttpWebRequest request = WebRequest.Create(ServerURL + "alarm/" + alarmGuid) as HttpWebRequest;
            request.Method = "PUT";
            request.Credentials = creds;
            if (UsePutCompression == PutRequestCompression.gzip)
                request.Headers.Add(HttpRequestHeader.ContentEncoding, "gzip");

            if (UsePutCompression == PutRequestCompression.deflate)
                request.Headers.Add(HttpRequestHeader.ContentEncoding, "deflate");

            using (Stream requestStream = request.GetRequestStream())
            {
                if (UsePutCompression == PutRequestCompression.none)
                    xml.Save(requestStream);
                else
                {
                    byte[] bytes = Encoding.Default.GetBytes(xml.OuterXml);

                    if (UsePutCompression == PutRequestCompression.gzip)
                    {
                        using (var zipStream = new System.IO.Compression.GZipStream(requestStream, System.IO.Compression.CompressionMode.Compress))
                        {
                            zipStream.Write(bytes, 0, bytes.Length);
                        }
                    }

                    if (UsePutCompression == PutRequestCompression.deflate)
                    {
                        using (var zipStream = new System.IO.Compression.DeflateStream(requestStream, System.IO.Compression.CompressionMode.Compress))
                        {
                            zipStream.Write(bytes, 0, bytes.Length);
                        }
                    }
                }
            }
            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Check the response
                    // WebException may be thrown already for some of this already like timeout or 404
                    if (request.HaveResponse == true && response == null)
                    {
                        Console.WriteLine("Response was not returned or is null");
                    }
                    else if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Console.WriteLine("PUT Request sent successfully");
                    }
                    else
                    {
                        Console.WriteLine("There was an error sending PUT Request");
                    }
                    Console.WriteLine("Response with status: " + response.StatusCode + " " + response.StatusDescription);

                    string responseContent = null;
                    // Get the response content
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                    Console.WriteLine("Reponse: " + responseContent);
                }
            }
            catch (WebException WebEx)
            {
                HttpWebResponse ExResponse = (HttpWebResponse)WebEx.Response;

                Console.WriteLine("Error while communicating with server");
                Console.WriteLine(WebEx.Message);
                if (ExResponse != null)
                {
                    Console.WriteLine("Status Code: {0}", (ExResponse).StatusCode);
                    Console.WriteLine("Status Description: {0}", (ExResponse).StatusDescription);

                    using (Stream ExDataStream = ExResponse.GetResponseStream())
                    {
                        using (StreamReader ExReader = new StreamReader(ExDataStream))
                        {
                            string ExResponseFromServer = ExReader.ReadToEnd();

                            Console.WriteLine("HTTP Response Body: {0}", ExResponseFromServer);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

        }
    }
}
