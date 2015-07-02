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

select * from sys.objects where type = 'u' and is_ms_shipped = 0

select 'node_command', * from node_command
select 'crawl_sequence', * from crawl_sequence
select 'command_function', * from command_function

