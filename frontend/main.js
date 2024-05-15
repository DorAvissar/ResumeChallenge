window.addEventListener('DOMContentLoaded', (event) => {
    getVisitCount();
})

const functionAPI = "https://getresumecounter2.azurewebsites.net/api/GetResumeCounter?code=0mIlv0NCxBzytyosP-dyl_4CFgMRDnE6N_EcZ0lsZNaUAzFu0yL00w==";

const getVisitCount = () => {
    let count = 30; // Default count in case of error

    fetch(functionAPI)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            console.log("Function API called successfully");
            count = data.count;
            document.getElementById("counter").innerText = count;
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });

    return count; // This will return the default count immediately, not waiting for the fetch operation
}
