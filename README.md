# YWV4
## 1. DB DDL

- Actions table

```
CREATE TABLE `actions` (
  `cmd_id` int(4) NOT NULL AUTO_INCREMENT,
  `pid` varchar(50) COLLATE utf8_bin NOT NULL,
  `param1` varchar(50) COLLATE utf8_bin NOT NULL,
  `param2` varchar(400) COLLATE utf8_bin NOT NULL,
  `param3` varchar(50) COLLATE utf8_bin NOT NULL,
  PRIMARY KEY (`cmd_id`),
  UNIQUE KEY `pid` (`cmd_id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8 COLLATE=utf8_bin
```

- Devices info table

```
CREATE TABLE `link_device` (
  `id` int(4) NOT NULL AUTO_INCREMENT,
  `pid` varchar(50) COLLATE utf8_bin NOT NULL,
  `hostname` varchar(100) COLLATE utf8_bin NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `pid` (`pid`),
  UNIQUE KEY `id` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=utf8 COLLATE=utf8_bin
```
