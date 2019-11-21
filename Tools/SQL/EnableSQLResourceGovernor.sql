-- check to see if the Workgroups AX_BATCH_WORKGROUP and AX_ONLINE_WORKGROUP are configured
SELECT * FROM SYS.DATABASE_RESOURCE_GOVERNOR_WORKLOAD_GROUPS
 
-- if not prepare the Default Settings regarding the workloadgroup settings
UPDATE axclassificationtable SET CONFIGUREDDATETIME = NULL
 
-- This setting increases the logfileusage
-- if loadtest runs fine, this must be applied in production
UPDATE axclassificationtable 
  SET MINCPUPERCENT = 20,
   MINLOGIOPERCENT = 20,
   MINIOPSPERCENT = 20,
   MAXCPUPERCENT = 100,
   MAXIOPSPERCENT = 100,
   MAXLOGRATEPERCENT = 100
  WHERE WORKLOADGROUPNAME = 'default'
UPDATE axclassificationtable 
  SET MINCPUPERCENT = 60,
   MINLOGIOPERCENT = 50,
   MINIOPSPERCENT = 60,
   MAXCPUPERCENT = 80,
   MAXIOPSPERCENT = 80,
   MAXLOGRATEPERCENT = 80
  WHERE WORKLOADGROUPNAME = 'AX_ONLINE_WORKGROUP'
UPDATE axclassificationtable 
  SET MINCPUPERCENT = 20,
   MINLOGIOPERCENT = 30,
   MINIOPSPERCENT = 20,
   MAXCPUPERCENT = 70,
   MAXIOPSPERCENT = 70,
   MAXLOGRATEPERCENT = 70
  WHERE WORKLOADGROUPNAME = 'AX_BATCH_WORKGROUP'
 
-- Apply the workgroups and enable RG
EXEC AXConfigureResourceGovernance
