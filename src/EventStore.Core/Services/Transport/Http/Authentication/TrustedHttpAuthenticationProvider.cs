// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

using System.Collections.Generic;
using System.Security.Claims;
using EventStore.Plugins.Authentication;
using Microsoft.AspNetCore.Http;

namespace EventStore.Core.Services.Transport.Http.Authentication {
	public class TrustedHttpAuthenticationProvider : IHttpAuthenticationProvider {
		public string Name => "trusted";

		public bool Authenticate(HttpContext context, out HttpAuthenticationRequest request) {
			request = null;
			if (!context.Request.Headers.TryGetValue(SystemHeaders.TrustedAuth, out var values)) {
				return false;
			}
			request = new HttpAuthenticationRequest(context, null, null);
			var principal = CreatePrincipal(values[0]);
			if (principal != null)
				request.Authenticated(principal);
			else
				request.Unauthorized();
			return true;
		}

		private ClaimsPrincipal CreatePrincipal(string header) {
			var loginAndGroups = header.Split(';');
			if (loginAndGroups.Length == 0 || loginAndGroups.Length > 2)
				return null;
			var login = loginAndGroups[0];
			var claims = new List<Claim>() {
				new Claim(ClaimTypes.Name, login)
			};
			if (loginAndGroups.Length == 2) {
				var groups = loginAndGroups[1];
				var roles = groups.Split(',');
				for (var i = 0; i < roles.Length; i++)
					claims.Add(new Claim(ClaimTypes.Role, roles[i].Trim()));

			}
			return new ClaimsPrincipal(new ClaimsIdentity(claims, SystemHeaders.TrustedAuth));
		}
	}
}
