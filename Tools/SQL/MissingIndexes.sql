select  DB_NAME(database_id) as DataBaseName,
			OBJECT_NAME(object_id, database_id) AS Tabelle,
			unique_compiles, user_seeks,
			avg_total_user_cost, avg_user_impact,
			(user_seeks+user_scans)*avg_total_user_cost*avg_user_impact/100 AS IMPACT,
			'create index [ix_'+ OBJECT_NAME(object_id,database_id) + '_' +
			convert(varchar,mid.index_handle)+ '] on ' + statement + '('+
			CASE when equality_columns IS not null then equality_columns ELSE '' END +
			CASE when equality_columns is not null and inequality_columns IS not null then ', ' else '' end +
			CASE when inequality_columns is not null then inequality_columns +')' else ')' end +
			CASE when included_columns IS not null then ' INCLUDE (' + included_columns + ')' ELSE '' end
			AS Kommando
			,*
from   sys.dm_db_missing_index_details mid
			 join sys.dm_db_missing_index_groups igs
					on mid.index_handle = igs.index_handle
			 join sys.dm_db_missing_index_group_stats migs
					on migs.group_handle = igs.index_group_handle
order  by IMPACT desc
