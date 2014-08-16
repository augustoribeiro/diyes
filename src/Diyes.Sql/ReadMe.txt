- Script to create Events table

create table Events
(
[Identity] nvarchar(50) not null,
[Version] int not null,
[Data] nvarchar(max) not null,
Primary key([Identity],Version)
)