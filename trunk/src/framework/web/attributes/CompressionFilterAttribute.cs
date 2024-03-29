﻿using System;
using System.IO.Compression;
using System.Web.Mvc;

namespace DL.Framework.Web
{
	internal class CompressionFilter : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var request = filterContext.HttpContext.Request;
			var acceptEncoding = request.Headers["Accept-Encoding"];
			if (String.IsNullOrEmpty(acceptEncoding))
				return;
			acceptEncoding = acceptEncoding.ToUpperInvariant();
			var response = filterContext.HttpContext.Response;
			if (acceptEncoding.Contains("GZIP"))
			{
				response.AppendHeader("Content-encoding", "gzip");
				response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
			}
			else if (acceptEncoding.Contains("DEFLATE"))
			{
				response.AppendHeader("Content-encoding", "deflate");
				response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
			}
		}
	}
}
