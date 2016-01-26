angular.module('people1', [])
.controller('HighSchool', function () {

    var nerds = [people[0],people[1]];

    var jocks = [people[3], people[4]];

    var preps = [people[2]];

    var people =[
        {
            id : 1,
            firstname : "Billy",
            lastname : "Jean"
        },
           {
               id: 2,
               firstname: "Tom",
               lastname: "Green"
           },
              {
                  id: 3,
                  firstname: "Guy",
                  lastname: "Richie"
              },
                 {
                     id: 4,
                     firstname: "Angel",
                     lastname: "Dean"
                 },
                    {
                        id: 5,
                        firstname: "Richard",
                        lastname: "Wilkes"

                    },
    ]
})