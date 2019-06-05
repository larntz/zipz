# zipz
Zip log/txt/csv files in specified directory.  Can group by day, month, or year. 

# usage
```
> .\zipz.exe --help
Zip files in specified directory.

*** SOURCE FILES WILL BE DELETED ***

Default options `--archive file --extension log --skip 7`

Usage:  [options] <Path>

Arguments:
  Path                  Path to directory containing files.

Options:
  -e|--extension <EXT>  File extension to zip. Accepts txt, csv, or log. Example: '-e txt'
  -a|--archive          Create one archive per time period. Accepts file, day, month, or year.
  -s|--skipdays <INT>   Skip the previous <INT> days when processing files. Example: -s 30 = ignore files less than 30 days old.
  -?|-h|--help          Show help information

```

# example
```
> .\zipz.exe -a month C:\exclogs\


##### ZIPZ #####

Source:         C:\exclogs\
Extension:      log
Archive per:    month
Skip days:      7



month-201904-created-2019-06-05T12_56_19.zip:      26 files compressed.
month-201905-created-2019-06-05T12_56_19.zip:      14 files compressed.
```
