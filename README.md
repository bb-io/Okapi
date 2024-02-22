# Blackbird.io Okapi longhorn

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->  

Longhorn is a server application that allows you to execute Batch Configurations remotely on any set of input files. Batch Configurations which include pre-defined pipelines and filter configurations, can be exported from Rainbow.

## Connecting

1. Navigate to apps and search for Okapi Longhorn. If you cannot find Okapi Longhorn, click **Add App** in the top right corner, select Okapi Longhorn, and add the app to your Blackbird environment.
2. Click **Add Connection**.
3. Name your connection for future reference, e.g., 'My Client'.
4. Go to your Okapi Longhorn server.
5. Copy its URL, for example: `http://28.216.252.148:88/okapi-longhorn`.
6. Paste it into the appropriate field in Blackbird.
7. Click **Connect**.
8. Confirm that the connection has appeared and the status is **Connected**. If this stage fails, ensure that you do not have a firewall blocking the connection, or configure your firewall to allow traffic from the Blackbird server.

## Actions



#### Upload files
- **Upload files**: Uploads files to a specific project. This action will upload each file provided in the request, handling them based on their file types, including special handling for ZIP files.

#### Projects
- **Create project**: Creates a new project within the system, returns the ID of the newly created project, and uploads a batch configuration file. This process involves making a POST request to the projects endpoint, extracting the project ID from the response's Location header, and subsequently uploading the batch configuration file provided in the request.
- **Execute project** Executes the batch configuration on the uploaded input files, returns the output files

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
