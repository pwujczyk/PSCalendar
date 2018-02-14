#Get-Calendar -AddSyncAccount "pwujczyk@gmail.com"
#Get-Calendar -Add "Event1CategorySport12:12" -Type Sport -StartDate "2018.02.12 12:12:00"
#Get-Calendar -Add "Event2CategoryFriends12:12" -Type Friends -StartDate "2018.02.13 12:12:00" -Duration 2h
#Get-Calendar -SyncAccount pwujczyk@gmail.com


Get-Calendar -Add "test1" -Type Friends -StartDate "2018.02.12 15:00"
Get-Calendar -SyncCurrentMonthAllAccounts
#delete in one account
Get-Calendar -SyncCurrentMonthAllAccounts