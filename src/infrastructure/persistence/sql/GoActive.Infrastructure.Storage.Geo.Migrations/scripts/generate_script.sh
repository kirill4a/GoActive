#!/bin/bash


cd ../
dotnet ef migrations script --context GeoContext --idempotent --output ./scripts/migrate.sql