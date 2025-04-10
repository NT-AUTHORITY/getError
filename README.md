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

## Download
Use [the latest version successfully built by Actions](https://github.com/NT-AUTHORITY/getError/actions)  
or [GitHub Releases](https://github.com/NT-AUTHORITY/getError/releases).  

> [!TIP]
> The `Actions` versions are built automatically every commit. Its usability is lower than the `Releases` versions.  
> On the other hand, the `Actions` version is the newest public build, while the `Releases` version is published manually and may be outdated.
> Select the version you want.
