# RTL Maze

To avoid having to install Visual Studio, you can use the Dockerfile to build
a working container that you can then run.

## API

The scraper starts automatically with the application.

The app is fully functional while the scraper is running, but not all shows
will be available. Only once the scraper has completed will the data be
fully complete.

*Scraper Status*

```
http://<site>/api/scraper
```

Note that the values displayed here are indicative only as the order of
processing is indeterminate.

*List shows*

```
http://<site>/api/shows/{page}
```

where `page` is zero indexed.

This app uses the same pagination scheme as the tvmaze api:

> This endpoint is paginated, with a maximum of 250 results per page.
> The pagination is based on show ID, e.g. page 0 will contain shows with
> IDs between 0 and 250. This means a single page might contain less than
> 250 results, in case of deletions, but it also guarantees that deletions
> won't cause shuffling in the page numbering for other shows.

## Implementation Notes

* Shows are stored in memory
* Storage is abstracted to allow easy future replacement with a DB for persistent storage
* Logging has been omitted for brevity
