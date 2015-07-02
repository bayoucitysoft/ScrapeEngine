if exists(select * from sys.objects where name ='node_command')
drop table node_command

create table node_command 
(
	id bigint identity(1,1),
	name nvarchar(256),
	description nvarchar(256),
	xpath nvarchar(max),
	first_cmd_id bigint, --we may not actually need this. It might be possible to derive command functionality based upon xpath ex: //a = inner text; 
	second_cmd_id bigint,
	domain_object_name nvarchar(256),
	domain_object_property nvarchar(256)
)
if exists(select * from sys.objects where name = 'command_function')
drop table command_function

create table command_function
(
	id bigint identity(1,1),
	operation nvarchar(50),
	attribute nvarchar(50),
	property nvarchar(256),
	value nvarchar(256)
)
/* Command functions are intended to be ran on individual nodes 
These should able to remain pretty global as their hard coded directions to the c# application */
insert into command_function 
(
	operation, 
	property,
	value 
)
values
('SelectNodes', null, null),
('SelectSingleNode', 'InnerText', null),
('SelectSingleNode', 'FirstChild', null),
('SelectSingleNode', 'Attribute', 'href'),
('SelectSingleNode', 'Text', null),
('SelectSingleNode', 'OuterHtml', null)
/* This table can be configued with any database, this example works with hubski.com as of 06.15.2015 */
insert into node_command
(
	name,
	description,
	xpath,
	first_cmd_id,
	second_cmd_id,
	domain_object_name,
	domain_object_property
)
--//div[@class=''unit'']//div[]"]
values
('prn', 'post root node',		'//div[@id=''unit'']',											1, null,	 null, null),
('ptn', 'post title node',		'.//div[@class=''feedtitle'']//span//a//span',					2, null,	'Hubski',		'Title'),
('ran', 'reply author node',	'.//span[@id=''username'']//a',									2, null,	'Hubski-Reply', 'Author'),
('pan', 'post author node',		'.//div[@class=''titlelinks'']//span[@id=''username'']//a',		3, 5,		'Hubski',		'Author'),
--('psn',	'score node',			'//div[@class=''plusminus''//span[@=class=''score''//a',	4, null,	'Hubski',		'Score'),
('ptno', 'post tag node one',	'.//div[@class=''subtitle'']//span[@id=''tag1'']//a',			2, null,	'Hubski',		'Tag1'),
('ptnt', 'post tag node two',	'.//div[@class=''subtitle'']//span[@id=''tag2'']//a',			2, null,	'Hubski',		'Tag2'),
('prl', 'post reply link',		'.//div[@class=''feedtitle'']//span/a',							4, null,	'Hubski',		'Url')
--('ddu', 'drill down url',		
--('gtp', 'generate time posted ago', null', null)


if exists(select * from sys.objects where name = 'crawl_sequence')
drop table crawl_sequence 
create table crawl_sequence
(
	id bigint identity(1,1),
	parent_id bigint, 
	site_name nvarchar(max),
	site_url nvarchar(max),
	sequence int,
	is_current bit default 0,
	is_active bit default 1,
	is_root bit default 0,
	crawl_depth int default 0,
	initial_node_command_id bigint,
	config_name nvarchar(256)
)

insert into crawl_sequence 
(parent_id, site_name, site_url, sequence, is_current, is_active, is_root, crawl_depth, initial_node_command_id, config_name)
values
(null, 'Hubski', 'http://www.hubski.com/', 1, 0, 1, 1, 1, 1 ,'Hubski'),
(1, 'HubskiLink1', N'/?id=228235', 2, 0, 1, 0, 0, null ,'Hubski-Reply'),
(1, 'HubskiLink2', N'/?id=20353', 3, 0, 1, 0, 0, null ,'Hubski-Reply')

if exists(select * from sys.objects where name = 'domain_object')
drop table domain_object

create table domain_object 
(
	id bigint identity(1,1),
	name nvarchar(256),
	property_name nvarchar(256),
	operator nvarchar(256),
)

insert into domain_object 
(name, property_name, operator)
values
('Hubski', 'Id', 'int'),			('Hubski', 'Score', 'int'),
('Hubski', 'Title', 'string'),		('Hubski', 'Subtitle', 'string'),
('Hubski', 'Author', 'string'),		('Hubski', 'Posted', 'DateTime'),
('Hubski', 'Url',	'string'),		('Hubski', 'Replies', 'List<Replies>'),
('Hubski', 'Content', 'string'),	('Hubski', 'Tag1', 'string'),
('Hubski', 'Tag2', 'string'),

('Hubski-Reply', 'Id', 'int'),			('Hubski-Reply', 'Author', 'string'),
('Hubski-Reply', 'Content', 'string'),	('Hubski-Reply', 'Replies', 'List<Reply>'),
('Hubski-Reply', 'Score', 'int')

if exists(select * from sys.objects where name = 'config')
drop table config
create table config
(
	id bigint identity(1,1),
	name nvarchar(256),
	node_command_id bigint,
	domain_object_id bigint
)

insert into config 
(
	name,
	node_command_id,
	domain_object_id
)
values 
('Hubski', 1, 1), ('Hubski-Reply', 3, 11), ('Hubski', 4, 5), ('Hubski', 5, 14), ('Hubski', 6, 10),
('Hubski', 7, 10), ('Hubski', 2, 3)





