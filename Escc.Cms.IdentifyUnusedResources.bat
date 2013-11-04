@echo off
cmsresourcedependencies.exe > example-report.txt
Escc.Cms.IdentifyUnusedResources.exe example-report.txt destination-email-address@example.org