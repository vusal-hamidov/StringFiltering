# StringFiltering

Lightweight .NET solution to upload large text chunks, process them asynchronously with pluggable string similarity strategies, and retrieve results.
Project is built using **.NET 9**, leveraging the latest features and performance improvements from the .NET platform.

## How it works (overview)

1. `UploadController` accepts input and stores them into `InMemoryChunkStorage`.
2. When a request arrives with IsLastChunk = true, the `UploadService` assembles all previously uploaded chunks for that `uploadId` into a single full text string and send it to `ConcurrentFilteringQueue`
3. `FilteringBackgroundService` dequeues chunks and uses `TextFilter` to run strategies.
4. Strategies implement `ITextFilterStrategy` (`Levenshtein`, `JaroWinkler`) in `StringFiltering.Domain`.
5. Results are stored in `InMemoryResultStore` (replaceable with persistent store).

## ⚙️ Project structure 
- `src/` 
  - `APIs/`
    - `StringFiltering.API` — ASP.NET Core presentation
  - `Core/`
    - `StringFiltering.Application` — use cases and orchestration
    - `StringFiltering.Domain` — business rules and strategies
    - `StringFiltering.Infrastructure` — background processing, queue, storage
- `tests/`
  - `StringFiltering.UnitTests` — unit tests for strategies, services, strategies etc.

## Build and run (Windows / CLI)

1. Restore and build:
    - `dotnet restore`
    - `dotnet build`

2. Run the API (from solution root):
    - `dotnet run --project StringFiltering.API`

The API will start on the configured port 5000 (open in browser `http://localhost:5000/swagger`).

## Configuration

The filter configuration is located in the Filtering section of the `appsettings.json` file.

````json
{
  "Filtering": {
    "Threshold": 0.8,
    "BadWords": [
      "unlucky",
      "terrible",
      "awfull",
      "nastry",
      "poor",
      "hatefull"
    ]
  }
}
````
Parameters:
   - `Threshold — a number between 0 and 1, setting the similarity threshold to classify a word as "bad". The higher the value, the stricter the filter.`
   - `BadWords — a list of words considered unacceptable and will be filtered out.`

# 📤 Upload endpoint

Endpoint to upload a chunk of a larger text payload for asynchronous processing.

### Endpoint

- `POST /api/upload`
- Controller file: `StringFiltering.API/Features/Upload/UploadController.cs`
- Content-Type: `application/json`

### Request body (example schema)

Use a JSON body matching `UploadChunkRequest`. Typical fields:

- `uploadId` (string) — client-generated id for the whole upload.
- `chunkIndex` (int) — zero-based index of this chunk.
- `data` (string) — chunk text.
- `isLastChunk` (bool) — last chunk or not.

Example request:
```json
{
  "uploadId": "123e4567-e89b-12d3-a456-426614174000",
  "chunkIndex": 0,
  "data": "First part of the large text...",
  "isLastChunk": true
}
````

````bash
curl -X POST "http://localhost:5000/api/upload" \
-H "Content-Type: application/json" \
-d '{
"uploadId":"123e4567-e89b-12d3-a456-426614174000",
"chunkIndex":1,
"data":"First part of the large text...",
"isLastChunk":false
}'
````
# 📊 Result endpoint

This controller exposes two HTTP endpoints:

- `Check the processing status of an upload`
- `Retrieve the final filtering result once processing is complete`

#### GET /api/status/{uploadId} — Check processing status
````bash
curl -X GET "http://localhost:5000/api/status/123e4567-e89b-12d3-a456-426614174000"
````

Example response
```json
{
  "uploadId": "123e4567-e89b-12d3-a456-426614174000",
  "status": "Processing"
}
```
```json
{
  "uploadId": "123e4567-e89b-12d3-a456-426614174000",
  "status": "Completed"
}
```

#### GET /api/result/{uploadId} — Get final filtered result
````bash
curl -X GET "http://localhost:5000/api/result/123e4567-e89b-12d3-a456-426614174000"
````
Example response
```json
{
  "uploadId": "123e4567-e89b-12d3-a456-426614174000",
  "status": "Completed",
  "filteredText": "large filtered text..."
}
```

## 🏃‍♂️ Fast Test endpoint (only for demonstration)

The API exposes a quick test endpoint to immediately check the program’s behavior without going through the full upload and processing flow.
This endpoint calls the upload service directly and returns a fast result, useful for quick verification or debugging.

#### GET /api/just-test

```bash
curl -X GET "http://localhost:5000/api/just-test"
````
After using the upload endpoints or the fast test endpoint, you can check the processing status and the filtered result using the Result endpoints:
```bash
curl -X GET "http://localhost:5000/api/status/5ae6cc07-f881-469e-a0f4-637644077edb"
curl -X GET "http://localhost:5000/api/result/5ae6cc07-f881-469e-a0f4-637644077edb"
```



## 🧪 Tests

To execute all unit tests, run the following command from the solution root:
```bash
dotnet test
```