To test the internals of the ActiveSiteMap class, build the
landscape module and its test module with the preprocessor
symbol "TEST_INTERNALS":

  C:\...> nant build -D:config=all -D:csc-define=TEST_INTERNALS
