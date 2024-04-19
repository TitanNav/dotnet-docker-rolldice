Basic dice-rolling .NET app instrumented with OpenTelemetry reporting to a collector, which is reporting to NR.

Environment variables used:

OTEL_EXPORTER_OTLP_ENDPOINT=https://otlp.nr-data.net

NEW_RELIC_LICENSE_KEY=<api_key_here>


Steps followed:

From dir rolldice:
```
dotnet publish -c Release
docker build -t rolldice3
```

From dir rolldice-with-collector:
```
docker pull otel/opentelemetry-collector-contrib:0.98.0
docker compose up
```
