var db = firebase.firestore();

function storeData() {

    
    var firstName = document.getElementById("firstName").value;
    var lastName = document.getElementById("lastName").value;
    var location = document.getElementById("location").value;
    var description = document.getElementById("description").value;
    var mobileNumber = document.getElementById("mobileNumber").value;
    var startTime = document.getElementById("startTime").value;
    var endTime = document.getElementById("endTime").value;
    var additionalInfo = document.getElementById("additionalInfo").value;

    db.collection("forms").doc(firstName).set({
        firstName: firstName,
        lastName: lastName,
        location: location,
        description: description,
        mobileNumber: mobileNumber,
        startTime: startTime,
        endTime: endTime,
        additionalInfo: additionalInfo
    })
        .then(function () {
            console.log("Doc successful");
        })
        .catch(function (error) {
            console.error("Error writing doc", error);
        });


}