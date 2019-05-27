using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace zProject.zHelper
{
    public class WebHelper
    {
        /// <summary>
        /// 无验证有参数的post
        /// </summary>
        /// <returns>服务器返回的数据</returns>
        /// <param name="postUrl">post网址</param>
        /// <param name="postData">需要的参数</param>
        public static string RequestDate(string postUrl, string postData)
        {
            //构建request
            WebRequest myHttpWebRequest = WebRequest.Create(postUrl);
            myHttpWebRequest.Method = "POST";
            myHttpWebRequest.ContentType = "application/json;charset=UTF-8";
            //处理需要发送的参数
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byte1 = encoding.GetBytes(postData);
            myHttpWebRequest.ContentLength = byte1.Length;
            Stream newStream = myHttpWebRequest.GetRequestStream();
            newStream.Write(byte1, 0, byte1.Length);
            newStream.Close();
            //发送成功后接收返回的XML信息
            HttpWebResponse response = (HttpWebResponse)myHttpWebRequest.GetResponse();
            string lcHtml = string.Empty;
            Encoding enc = Encoding.GetEncoding("UTF-8");
            Stream stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream, enc);
            lcHtml = streamReader.ReadToEnd();
            return lcHtml;
        }
        /// <summary>
        /// 有验证无参数的post request
        /// </summary>
        /// <returns>服务器返回的数据</returns>
        /// <param name="postUrl">post网址</param>
        /// <param name="postToken">token令牌</param>
        public static string ARequestDate(string postUrl, string postToken)
        {
            //构建request
            WebRequest myHttpWebRequest = WebRequest.Create(postUrl);
            myHttpWebRequest.Method = "POST";
            //在header中添加token
            myHttpWebRequest.Headers.Add("Authorization", postToken);
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            //必须有长度
            myHttpWebRequest.ContentLength = 0;
            //发送成功后接收返回的XML信息
            HttpWebResponse response = (HttpWebResponse)myHttpWebRequest.GetResponse();
            string lcHtml = string.Empty;
            Encoding enc = Encoding.GetEncoding("UTF-8");
            Stream stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream, enc);
            lcHtml = streamReader.ReadToEnd();
            return lcHtml;

        }
        /// <summary>
        /// 有验证有参数的post
        /// </summary>
        /// <returns>服务器返回的数据</returns>
        /// <param name="postUrl">post网址</param>
        /// <param name="postData">需要的参数</param>
        /// <param name="postToken">token令牌</param>
        public static string BRequestDate(string postUrl, string postData, string postToken)
        {
            WebRequest myHttpWebRequest = WebRequest.Create(postUrl);
            myHttpWebRequest.Method = "POST";
            myHttpWebRequest.Headers.Add("Authorization", postToken);
            myHttpWebRequest.ContentType = "application/json;charset=UTF-8";

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byte1 = encoding.GetBytes(postData);
            myHttpWebRequest.ContentLength = byte1.Length;
            Stream newStream = myHttpWebRequest.GetRequestStream();
            newStream.Write(byte1, 0, byte1.Length);
            newStream.Close();

            //发送成功后接收返回的XML信息
            HttpWebResponse response = (HttpWebResponse)myHttpWebRequest.GetResponse();
            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string lcHtml = string.Empty;
            Encoding enc = Encoding.GetEncoding("UTF-8");
            Stream stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream, enc);
            lcHtml = streamReader.ReadToEnd();
            return lcHtml;

        }

    }
}
