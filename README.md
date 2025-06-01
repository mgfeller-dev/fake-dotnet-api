# Fake DotNet API

The purpose of this application is to provide a walking skeleton that can be used as a baseline for
a [ASP.NET Core minimal API](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis).

It is also intended to be used to test platform related features.

## Build and run Docker image locally

Build the image using

```shell
docker build . -t fake-product/fake-dotnet-api:latest
```

and run using (replace example values if necessary)

```shell
docker run --network=host -p 8080:8080 fake-product/fake-dotnet-api:latest
```

## Test with `curl`:

```shell
curl 127.0.0.1:8080/healthcheck
```

returns response similar to

```json
{
  "message": "OK",
  "version": "0000000000000000000000000000000000000000",
  "status": "Healthy",
  "results": {
    "Default": {
      "status": "Healthy",
      "description": "The service is healthy",
      "data": {
        "VersionGitSHA": "0000000000000000000000000000000000000000",
        "VersionTag": "v0.0.0"
      }
    }
  }
}
```

```shell
curl -s -o /dev/null -w "%{http_code}" 127.0.0.1:8080
# expected to return HTTP status 404
```

```shell
curl -s -o /dev/null -w "%{http_code}" 127.0.0.1:8080/unauthorized
# expected to return HTTP status 401
```

```shell
curl 127.0.0.1:8080/info
```

returns response similar to

```json
{
  "versionTag": "v0.0.0",
  "versionGitSHA": "0000000000000000000000000000000000000000",
  "environment": "unknown",
  "applicationName": "QuoteApi"
}
```

## Upstream calls

The `upstream` endpoint executes GET calls (without authentication) to endpoints either defined
in `appsettings.json` or as environment variables.

For instance, in `appsettings.json`

```json
  "upstream": {
    "endpoints": {
      "httpbin": "https://httpbin.org/get",
      "httpbin-200": "https://httpbin.org/status/200"
    }
  },
```

or as environment variable(s)

```shell
export UPSTREAM__ENDPOINTS__HTTPBIN=https://httpbin.org/get
```

Executing `curl http://127.0.0.1:8080/upstream` will return something similar to:

```json
{
  "startTime": "2025-01-25 20:07:29",
  "endTime": "2025-01-25 20:07:30",
  "startTimeUtc": "2025-01-25 19:07:29",
  "endTimeUtc": "2025-01-25 19:07:30",
  "duration": 0.6029245,
  "upstreamCalls": [
    {
      "name": "HTTPBIN",
      "url": "https://httpbin.org/get",
      "responseCode": "OK",
      "responseHeaders": {
        "Date": "Sat, 25 Jan 2025 19:07:30 GMT",
        "Connection": "keep-alive",
        "Server": "gunicorn/19.9.0",
        "Access-Control-Allow-Origin": "*",
        "Access-Control-Allow-Credentials": "true",
        "Content-Type": "application/json",
        "Content-Length": "313"
      },
      "startTime": "2025-01-25 20:07:29",
      "endTime": "2025-01-25 20:07:30",
      "startTimeUtc": "2025-01-25 19:07:29",
      "endTimeUtc": "2025-01-25 19:07:30",
      "duration": 0.5948302,
      "responseBody": {
        "args": {},
        "headers": {
          "Accept": "application/json",
          "Host": "httpbin.org",
          "Traceparent": "00-7af71f762fb2bb19c4de6c9ef09ef7dd-f71f82a04a320d40-00",
          "X-Amzn-Trace-Id": "Root=1-67953672-248917bc3502b95e794b65e7"
        },
        "origin": "84.215.66.117",
        "url": "https://httpbin.org/get"
      },
      "exceptionMessage": ""
    }
  ]
}
```

## JWT config

Export the following environment variables, e.g. using a `direnv` `.envrc`-file:
```shell
# JWT Settings
export JwtSettings__Issuer="https://dev-b25uzcek87i4o2um.eu.auth0.com/"
export JwtSettings__Audience="api://fake-api-csharp"
``` 

## Quotes data

The application includes a `quotes` endpoint to store and retrieve quotes.

Depending on the implementation and configuration, this might be an in-memory database, or a persistent database.

```shell
# add a quote
curl -d '{"source":"anonymous", "text":"I hate quotes, tell me what you know"}' -H "Content-Type: application/json" -X POST http://127.0.0.1:8080/quotes

# add a random quote
curl -X POST http://127.0.0.1:8080/quotes/rand

# get quotes
curl http://127.0.0.1:8080/quotes 
```

## Swagger / OpenAPI

The API can be explored using the Swagger API explorer at http://localhost:8080/swagger/index.html.
