# Henry Meds Tech Assessment

This repository is used as a demonstrative CRUD app for software that manages appointments.

> [!NOTE]
> In an effort to timebox this assessment, I have left 'main' as my timeboxed branch, and intend
> to add comments to another branch.

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

### Tradeoffs And Assumptions

There were many tradeoffs in this implementation of the assigned work.

Most notably, there is almost no data validation to speak of. This was cut for time constraints, and theoretically the work could remain in place if this service did not exist at the edge, and instead utilized an API service layer.

For example:
* A client can create an availability, and a provider can request an appointment.
* There is also no resolution for the time availabilities, so a user can request an appointment 8 minutes, 12 seconds past the hour.
* A user can set multiple availabilities that overlap. There is no check to see if a requested availability has already been set.
