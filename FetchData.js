//fetch('api/SampleData/WeatherForecasts')

fetch('api/TasksGroups/Groups')
     //fetch('api/SampleData/WeatherForecasts')
      .then(response => response.json())
      .then(data => {
        this.setState({ groups: data, loading: false });
      });