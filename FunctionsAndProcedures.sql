go
--return root url 
if exists(select * from sys.objects where name = 'NextCrawlSequenceId' and type = 'fn')
drop function NextCrawlSequenceId

go
create function NextCrawlSequenceId()
returns bigint
as
begin

declare @id bigint = (
	select top 1 cs.id
	from crawl_sequence cs 
	where cs.is_active = 1 
		and cs.is_current = 0
	order by sequence
)
	return @id
end 

go -- crawl sequence function test 
declare @id bigint
select @id = dbo.NextCrawlSequenceId()
select @id
go 

if exists(select * from sys.objects where name = 'NextCrawlSequence')
drop procedure NextCrawlSequence 
go
create procedure NextCrawlSequence
as
begin
	declare @crawl_sequence_id bigint = dbo.NextCrawlSequenceId()
	select 
	*
	from crawl_sequence where Id = @crawl_sequence_id
end
go
exec NextCrawlSequence 
go
if exists(select * from sys.objects where name = 'NodeCommandByCrawlSequenceId')
drop procedure NodeCommandByCrawlSequenceId
go
create procedure NodeCommandByCrawlSequenceId
(
	@crawl_sequence_id bigint 
)
as
begin
	select
	n.id,
	n.name,
	n.description,
	n.first_cmd_id,
	n.second_cmd_id,
	n.xpath,
	n.domain_object_name,
	n.domain_object_property
	from crawl_sequence c
	inner join node_command n on n.domain_object_name = c.config_name
	inner join command_function f with(nolock) on f.id = n.first_cmd_id
	left outer join command_function f2 with(nolock) on f2.id = n.second_cmd_id
	where c.id = @crawl_sequence_id
	and f.id <> isnull(c.initial_node_command_id, 0) 
end
go
exec NodeCommandByCrawlSequenceId 1 select * from crawl_sequence where id = 1
go 
if exists(select * from sys.objects where name = 'NodeCommandById')
drop procedure NodeCommandById
go
create procedure NodeCommandById
(
	@id bigint 
)
as
begin
	select 
	id,
	name,
	description,
	xpath,
	first_cmd_id,
	second_cmd_id,
	domain_object_name,
	domain_object_property
	from node_command
	where id = @id 
end
go
if exists(select * from sys.objects where name = 'CommandFunctionById')
drop procedure CommandFunctionById
go
create procedure CommandFunctionById
(
	@id bigint 
)
as
begin 
	select 
	id,
	operation,
	property,
	value
	from command_function
	where id = @id
end
go
exec CommandFunctionById 1
go
if exists(select * from sys.objects where name = 'CrawlSequenceAndUrl')
drop procedure CrawlSequenceAndUrl
go
create procedure CrawlSequenceAndUrl
as
begin
	select distinct 
	site_name,
	site_url
	from crawl_sequence c
	where parent_id is null 
end
go
if exists(select * from sys.objects where name = 'CurrentCrawlSequence')
drop procedure CurrentCrawlSequence 
go
create procedure CurrentCrawlSequence
as
begin
	select 
	site_name
	from crawl_sequence
	where is_current = 1 
end
go
exec CurrentCrawlSequence
go
if exists(select * from sys.objects where name = 'RetrieveNodeCommands')
drop procedure RetrieveNodeCommands
go
create procedure RetrieveNodeCommands
as
begin
	select 
	* 
	from node_command
end
go
if exists(select * from sys.objects where name = 'domain_objects')
drop view domain_objects 
go
create view domain_objects
as
select distinct
domain_object_name,
domain_object_property
from node_command
where domain_object_name is not null 
and domain_object_property is not null 
go
select * from domain_objects
go
if exists(select * from sys.objects where name = 'ValidateDomainObject')
drop procedure ValidateDomainObject 
go
create procedure ValidateDomainObject
(
	@domain_object_name nvarchar(256),
	@domain_object_property nvarchar(256)
)
as
begin
	if not exists(select * from sys.objects where name = @domain_object_name and type = 'u')
	begin 
		declare @sql nvarchar(max) = 
		'
			create table ' + @domain_object_name + '
			(
				id bigint identity(1,1)
			)
		'
		exec(@sql)
	end
	if not exists(select * from information_schema.columns where table_name = @domain_object_name and column_name = @domain_object_property)
	begin 
		declare @table_sql nvarchar(max) =
		'
			alter table ' + @domain_object_name + '
			add ' + @domain_object_property + ' nvarchar(256) 
		'
		exec(@table_sql)
	end
end
go
if exists(select * sys.objects where name = 'FlattenDomainObject')
drop procedure FlattenDomainObject
go
create procedure FlattenDomainObject
(
	@object_guid uniqueidentifier,
	@column_name nvarchar(256),
	@column_value nvarchar(max),
	@table_name nvarchar(256)
)
as
begin
	if not exists(select * from sys.objects where name = 'domain_cache_' + cast(@object_guid as nvarchar(50)))
	begin 
		declare @create_cache_table nvarchar(max) =
		'
			create table domin_cache_' + cast(@object_guid as nvarchar(50)) + '
			(
				id bigint identity(1,1)
			)
		'
		begin try 
		exec(@create_cache_table)

		end try
		begin catch
			
		end catch	
	end
	declare @alter_table_guid nvarchar(max) = 
	'
		alter table domain_cache_' + cast(@object_guid as nvarchar(50)) + '
		add column ' + @column_name + ' nvarchar(1024) 
	'
	exec(@alter_cache_table)
end
go

select 'node_command', * from node_command
select 'crawl_sequence', * from crawl_sequence
select 'command_function', * from command_function


