# ClassModelComparer

## Overview
This application compares a class model defined in ArgoUML (zargo file) with a class model defined by a .NET assembly (dll file). The motivation for the application was to compare an [ADAPT](https://adaptframework.org/) ArgoUML class model with its corresponding .NET assembly to determine the gaps.

## Output
The application produces two output files:
1. Text-file list of differences between the two models
2. Detailed XML file containing all captured details about the two class models as well as differences between them.

## How it Works
The application extracts the XMI file from the zargo file, which is just a zip archive. It builds a representation of the class model in memory. Then it uses reflection to build a representation of the .NET assembly class model in memory. Then it compares the the two and produces the output.

## How To Use It
Edit the config.xml file to specify correct paths to the zargo and dll files, and the correct path to an existing folder where the output should be produced. Then run it. It should take less than a second to run for a small model.

## Platform
I developed the code using Visual Studio for Mac. I have confirmed that it works fine using Visual Studio on Windows.

## External Dependencies
The application uses DotNetZip to extract the XMI file from the zargo file.

## Near-Term Development Plans
1. Fix known issues (see below).
2. Add support for comparing any combination of two ArgoUML and/or DLL files.

## Known Issues
1. The HTML output isn't produced. Loading the XSL file into the XslCompiledTransform fails. I have no idea what the problem is. It works fine doing the transformation using an XML tool like oXygen XML.
2. In the HTML output, links are created for types that are not classes in the output (e.g., primitive types). The links don't hurt anything, but they don't go anywhere.
3. In the HTML output, nullable, "List of x", and "IEnumerable of" types are not properly linked.
