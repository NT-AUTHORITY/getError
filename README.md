# getError
 A simple C# console program returns the error description as json.
 
## Usage
```bash
 .\getError.exe 0x00000000 NTSTATUS en_US  
 .\getError.exe 0x00000000 NTSTATUS en-US  
 .\getError.exe 0x00000000 NTSTATUS  
 .\getError.exe 0x00000000 WIN32  
 .\getError.exe 0x00000000 HRESULT  
 .\getError.exe 0x00000000 NTSTATUS  
```
`WIN32` `NTSTATUS` and `HRESULT` doesn't actually matter LOL

## Return
### Success
```bash
 PS D:\getError> .\getError.exe 0x00000000 NTSTATUS en_US
 {"status":1,"main":{"category":"NTSTATUS","code":"0x00000000","msg":"The operation completed successfully."},"ex":null}
```
### Error
```bash
 PS D:\getError> .\getError.exe 0xFFFFFFF NTSTATUS en_US
 {"status":0,"main":null,"ex":"Error description not found."}
```
