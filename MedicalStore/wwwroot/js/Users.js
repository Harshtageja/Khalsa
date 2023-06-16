var Users = Users || {};
Users.Searchitem = function () {
    const queryString = window.location.search;
    const urlParams = new URLSearchParams(queryString);
    const UserId = urlParams.get('UserId')
    var toSearch = document.getElementById("search").value;
    if (toSearch.length > 3) {
        $.ajax({
            type: "POST",
            url: "/User/SearchMedicine",
            dataType: "json",
            data: { "tosearch": toSearch },
            success: function (res) {
                console.log(res.length);
                document.getElementById("tableData").innerHTML = "";
                for (var i = 0; i < res.length; i++) {
                    var status = res[i].final > 0 ? "Available" : "Not Available"
                    $("#tableData").append("<tr><td>" + res[i].medicineName + "</td><td>" + status + "</td><td>" + res[i].price + "</td><td><a onclick='Users.OpenModal(" + res[i].id + "," + res[i].price +")'>View Details</a>" + "" + "</td></tr>");
                }
                //res.forEach(function (element) {
                //    $("#tableData").append("<tr><td>" + element.MedicineName + "</td><td>" + element.StockIn + "</td><td>" + element.StockOut + "</td><td>" + element.Expired + "</td><td>" + element.Final +"</tr>");
                //})
            }
        });
    }
    if (toSearch.length == 0) {
        $.ajax({
            type: "POST",
            url: "/User/ShowMeAllMedicine",
            dataType: "json",
            data: { "tosearch": toSearch },
            success: function (res) {
                console.log(res.length);
                document.getElementById("tableData").innerHTML = "";
                for (var i = 0; i < res.length; i++) {
                    var status = res[i].final > 0 ? "Available" : "Not Available"
                    $("#tableData").append("<tr><td>" + res[i].medicineName + "</td><td>" + status + "</td><td>" + res[i].price + "</td><td><a onclick='Users.OpenModal(" + res[i].id + "," + res[i].price + ")'>View Details</a>" + "" + "</td></tr>");
                }
                //res.forEach(function (element) {
                //    $("#tableData").append("<tr><td>" + element.MedicineName + "</td><td>" + element.StockIn + "</td><td>" + element.StockOut + "</td><td>" + element.Expired + "</td><td>" + element.Final +"</tr>");
                //})
            }
        });
    }
}
Users.OpenModal = function (medicine_id, UnitPrice) {
    document.getElementById("MedicineID").value = medicine_id;
    document.getElementById("UnitPrice").value = UnitPrice;
    $.ajax({
        type: "POST",
        url: "/User/GiveMecount",
        dataType: "json",
        data: { "MedicineID": medicine_id},
        success: function (res) {
            if (res != null) {
                if (res.length == 0) {
                    document.getElementById("count").innerText = 0;
                }
                else
                document.getElementById("count").innerText = res[0].counting;
            }
            else {
                alert("Something went Wrong");
            }
        }
    });
        $('#exampleModal').modal('show');

}
Users.OpenCartModal = function (medicine_id, UnitPrice) {
    document.getElementById("MedicineID").value = medicine_id;
    document.getElementById("UnitPrice").value = UnitPrice;
    $.ajax({
        type: "POST",
        url: "/User/GiveMecount",
        dataType: "json",
        data: { "MedicineID": medicine_id },
        success: function (res) {
            if (res != null) {
                if (res.length == 0) {
                    document.getElementById("count").innerText = 0;
                }
                else
                    document.getElementById("count").innerText = res[0].counting;
            }
            else {
                alert("Something went Wrong");
            }
        }
    });
    $('#exampleModal').modal('show');

}
    Users.Login = function () {
        var Email = document.getElementById("username").value;
        var Password = document.getElementById("password").value;
        $.ajax({
            type: "POST",
            url: "/User/LoginMeAsCustomer",
            dataType: "json",
            data: { "Email": Email,"Password":Password },
            //success: function (res) {
            //    console.log("hello");
            //    //if (res) {
            //    //    console.log("hello");
            //    //    Users.UserId = res.UserId;
            //    //    window.location.href = "https://localhost:44310/User/SearchingPage";
            //    //}
            //    //else {
            //    //    console.log("byr");
            //    //}
            //    //res.forEach(function (element) {
            //    //    $("#tableData").append("<tr><td>" + element.MedicineName + "</td><td>" + element.StockIn + "</td><td>" + element.StockOut + "</td><td>" + element.Expired + "</td><td>" + element.Final +"</tr>");
            //    //})
            //}
            success: function (res) {
                console.log(res.length);
                for (var i = 0; i < res.length; i++) {
                    var status = res[i].final > 0 ? "Available" : "Not Available"
                    $("#tableData").append("<tr><td>" + res[i].medicineName + "</td><td>" + status + "</td><td>" + res[i].price + "</td><td><a onclick='Users.OpenModal(" + res[i].id + "," + Users.UserId + ")'>View Details</a>" + "" + "</td></tr>");
                }
                //res.forEach(function (element) {
                //    $("#tableData").append("<tr><td>" + element.MedicineName + "</td><td>" + element.StockIn + "</td><td>" + element.StockOut + "</td><td>" + element.Expired + "</td><td>" + element.Final +"</tr>");
                //})
            }

        });
}
Users.Test = function () {
    var Email = document.getElementById("username").value;
    var Password = document.getElementById("password").value;
    $.ajax({
        type: "POST",
        url: "/User/LoginMeAsCustomer",
        dataType: "json",
        data: { "Email": Email, "Password": Password },
        success: function (res) {
            console.log(res.length);
            Users.UserId = res[0].userId;
            window.location.href = "https://localhost:44310/User/SearchingPage?UserId="+Users.UserId;
            //res.forEach(function (element) {
            //    $("#tableData").append("<tr><td>" + element.MedicineName + "</td><td>" + element.StockIn + "</td><td>" + element.StockOut + "</td><td>" + element.Expired + "</td><td>" + element.Final +"</tr>");
            //})
        }
    });
    
}
Users.AddtoCart = function () {
    var count = document.getElementById("count").innerText;
    var MedicineID = document.getElementById("MedicineID").value;
    var UnitPrice = document.getElementById("UnitPrice").value;
    $.ajax({
        type: "POST",
        url: "/User/AddtoCart",
        dataType: "json",
        data: { "MedicineId": MedicineID, "Count": count,"UnitPrice":UnitPrice },
        success: function (res) {
            if (res == "Success") {
                alert("Added to Cart Successfully");
                $('#exampleModal').modal('hide');
                location.reload();
            }
            else {
                alert("Something went Wrong");
            }
        }
    });

}
Users.changeStatus = function (orderId,FUTURE) {
    $.ajax({
        type: "POST",
        url: "/User/ChangeStatus",
        dataType: "json",
        data: { "orderId": orderId,"FUTURE":FUTURE},
        success: function (res) {
            if (res == "success") {
                location.reload();
            }
            else {
                alert("Something went Wrong");
            }
        }
    });
}
Users.MakeUser = function () {
    var Name = document.getElementById("inputName").value;
    var Phone = document.getElementById("inputPhone").value;
    var Email = document.getElementById("inputEmail4").value;
    var Password = document.getElementById("inputPassword4").value;
    var Address = document.getElementById("inputAddress").value +" " +document.getElementById("inputAddress2").value +" "+ document.getElementById("inputCity").value +" "+ document.getElementById("inputState").value +" " + document.getElementById("inputZip").value;
    $.ajax({
        type: "POST",
        url: "/User/MakeUser",
        dataType: "json",
        data: { "Name": Name, "Phone": Phone,"Email":Email ,"Password": Password,"Address": Address},
        success: function (res) {
            console.log("Hello");
            if (res == "success") {
                alert("User Created");
                window.location.href = "https://localhost:44310/StoreManager/CustomerLogin"
            }
            else if (res == "AlreadyExists") {
                alert("User Already Exists");
            }
            else {
                alert("Something went Wrong");
            }
        }
    });
}