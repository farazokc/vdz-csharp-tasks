sequenceDiagram
    participant User
    participant FileUploader
    participant RetryPolicy
    participant Chunk
    participant ProgressTracker
    participant StorageAccountConfig
    participant AzureBlobStorageAccountProvider

    User->>FileUploader: Specify file path and storage account config
    FileUploader->>StorageAccountConfig: Load configuration
    StorageAccountConfig->>AzureBlobStorageAccountProvider: Authenticate and get config
    FileUploader->>Chunk: Split file into chunks
    FileUploader->>ProgressTracker: Initialize progress tracker
    FileUploader->>FileUploader: Start uploading chunks one by one
    FileUploader->>RetryPolicy: Check if retry is needed
    RetryPolicy->>FileUploader: Retry chunk upload (if necessary)
    FileUploader->>ProgressTracker: Update progress
    FileUploader->>StorageAccountConfig: Upload chunk to Azure Blob Storage
    FileUploader->>ProgressTracker: Notify progress
    FileUploader->>User: Provide progress feedback
    FileUploader->>ProgressTracker: Finish upload
