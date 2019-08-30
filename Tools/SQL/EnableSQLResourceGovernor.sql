-- check to see if the Workgroups AX_BATCH_WORKGROUP and AX_ONLINE_WORKGROUP are configured
SELECT * FROM SYS.DATABASE_RESOURCE_GOVERNOR_WORKLOAD_GROUPS
 
-- if not prepare the Default Settings regarding the workloadgroup settings
UPDATE axclassificationtable SET CONFIGUREDDATETIME = NULL
 
-- This setting increases the logfileusage
-- if loadtest runs fine, this must be applied in production
UPDATE axclassificationtable SET MAXLOGRATEPERCENT = 60 WHERE WORKLOADGROUPNAME = 'AX_BATCH_WORKGROUP'
 
-- Apply the workgroups and enable RG
EXEC AXConfigureResourceGovernance