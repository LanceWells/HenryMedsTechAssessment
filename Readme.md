# Henry Meds Tech Assessment

This repository is used as a demonstrative CRUD app for a WMS.

## Setup

There are multiple options for setting this project up locally.

### Using Docker, Kubernetes, and Tilt

1. Ensure [Dotnet 7.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) is installed.
1. Ensure containerization tools (Docker, K8s) are installed ([Docker Desktop](https://www.docker.com/products/docker-desktop/) is a good option for this!).
1. Ensure [Tilt](https://docs.tilt.dev/install.html) is installed.

The app may be run via the following at the root of the project:
```
tilt up
```
The service will be available at port `3000`.

<br />

### Just using C#

1. Ensure [Dotnet 7.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) is installed.

The app may be run by invoking the `.Net Core Launch` debug option via VS Code.

The service will host an http endpoint at port `5145`, and an https endpoint at `7121`.