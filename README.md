# ODataCouchbase

OData api to Couchbase
<https://stackoverflow.com/q/61078681/1885735>

## Couchbase setup

Enable travel-sample sample bucket:
<https://docs.couchbase.com/server/current/manage/manage-settings/install-sample-buckets.html>

## Testing

Works: <http://localhost:5000/odata/airline?$filter=Country eq 'United Kingdom'>
Fails: <http://localhost:5000/odata/airline?$filter=Country eq 'United Kingdom'&$select=Id,Name>
