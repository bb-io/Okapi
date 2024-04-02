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

### XLIFF conversion actions

This combination of actions allows you to convert any Okapi-compatible file to XLIFF and back. Use it in combination with other Blackbird apps that take or modify XLIFF files.

- **Convert file to XLIFF** takes any Okapi-compatible file format and converts it to an XLIFF file. It outputs both an XLIFF and a 'package' file.
- **Convert XLIFF to file** takes a (modified) XLIFF file and a package file, it returns the original file.

### Manual batchconfig actions

Use the following actions to have full control of batchconfig executions on one or multiple files

- **Upload files**: Uploads files to a specific project. This action will upload each file provided in the request, handling them based on their file types, including special handling for ZIP files.
- **Create project**: Creates a new project within the system, returns the ID of the newly created project, and uploads a batch configuration file.
- **Execute project** Executes the batch configuration on the uploaded input files, returns the output files

## Example

![okapi-example-bird](image/README/okapi-example-bird.png)

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
