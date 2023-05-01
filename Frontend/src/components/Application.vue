<template>
  <div>
    <main class="box">
      <h2>Application</h2>
      <form @submit.prevent="onPost">
        <div class="inputBox">
          <label for="description">Description</label>
          <input type="text" class="form-control" v-model="description" id="description" placeholder="Description" required/>
        </div>
        <div class="inputBox">
          <label for="solveDate">Solve Date</label>
          <input type="text" class="form-control" id="solveDate" v-model="solveDate" />
          <div v-if="solveDateError" class="invalid-feedback">{{ solveDateError }}</div>
        </div>
        <button type="submit" class="btn btm primary">Submit application</button>
      </form>
    </main>
    <main class="box applications">
      <h2>Applications List</h2>
      <table>
        <thead>
          <tr>
            <th>Description</th>
            <th>Entry Date</th>
            <th>Solve Date</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="application in applications" :key="application.id" :class="{'due-soon': application.isDueSoon}">
            <td>{{ application.description }}</td>
            <td>{{ new Date(application.entryDate).toLocaleString('en-GB', { dateStyle: 'medium', timeStyle: 'short' }) }}</td>
            <td>{{ new Date(application.solveDate).toLocaleString('en-GB', { dateStyle: 'medium', timeStyle: 'short' }) }}</td>
            <td> <button v-if="!application.hasBeenCompleted" @click="onComplete(application.id)">Mark as Completed</button> </td>
          </tr>
        </tbody>
      </table>
    </main>
    <footer>
    </footer>
  </div>
</template>

<script>
import axios from 'axios';
import { createApp, ref, onMounted } from "vue";
import Flatpickr from 'flatpickr';
import 'flatpickr/dist/flatpickr.css';

export default {
  setup() {
    const description = ref('');
    const entryDate = ref('');
    const solveDate = ref('');
    const applications = ref([]);
    const solveDateError = ref('');

    const validateSolveDate = () => {  // Checks if solve date is valid(not in the past)
      const now = new Date();
      const selectedDate = new Date(solveDate.value);
      if (selectedDate < now) {
        solveDateError.value = 'Solve date cannot be in the past';
        return false;
      }
      solveDateError.value = '';
      return true;
    };

    const onPost = () => {
  entryDate.value = new Date().toISOString();
  if (!validateSolveDate()) {
    return; // Stop submitting the form if validation fails
  }
  const solveDateTime = new Date(solveDate.value).toISOString();
  axios.post('http://localhost:5166/api/Applications', {
    description: description.value,
    entryDate: entryDate.value,
    solveDate: solveDateTime
  })
    .then(() => { // Gets all the applications which have not been completed
      axios.get('http://localhost:5166/api/Applications')
        .then(response => {
          applications.value = response.data;
        })
        .catch(error => {
          console.log(error);
        });
    })
    .catch(error => {
      console.log(error);
    });
};

    const onComplete = (id) => { // Edits an application if user presses completed button
      axios.put(`http://localhost:5166/api/Applications/${id}`, {
        hasBeenCompleted: true
      })
        .then(() => { // Gets the applications which have not been completed
          axios.get('http://localhost:5166/api/Applications')
            .then(response => {
              applications.value = response.data.filter(a => !a.hasBeenCompleted);
            })
            .catch(error => {
              console.log(error);
            });
        })
        .catch(error => {
          console.log(error);
        });
    };

    onMounted(() => {
      const flatpickr = new Flatpickr('#solveDate', {
        enableTime: true,
        dateFormat: "Y/m/d H:i:S",
        minDate: "today", 
        onChange: (selectedDates, dateStr) => {
          solveDate.value = dateStr;
          validateSolveDate(); 
        }
      });

      flatpickr.open();
    });

    return {
      description,
      entryDate,
      solveDate,
      solveDateError,
      applications,
      onPost,
      onComplete,
      validateSolveDate
    };
  }
};
</script>



<style>
* {
  box-sizing: border-box;
}
body {
  font-family: sans-serif;
  height: 100vh;
  margin: 0;
  padding: 0;
  background-color: rgb(50, 50, 57);
  display: flex;
  justify-content: center;
  align-items: center;
}
header {
  display: none;
}
.box {
  background-color: rgba(0, 0, 0, 0.8);
  border-radius: 10px;
  box-shadow: 0 15px 25px rgba(0, 0, 0, 0.8);
  margin: 40px;
  padding: 40px;
  text-align: left;
  flex: 1;
}
.box:nth-child(1) {
  margin-right: 20px;
}
.box h2 {
  margin: 0 0 30px 0;
  padding: 0;
  color: #fff;
  text-align: center;
}
.box .inputBox label {
  color: #fff;
}
.box .inputBox input {
  background: transparent;
  border: none;
  border-bottom: 1px solid #fff;
  color: #fff;
  font-size: 18px;
  letter-spacing: 2px;
  margin-bottom: 30px;
  outline: none;
  padding: 10px 0;
  width: 100%;
}
.box input[type="submit"],
.box button[type="submit"],
a.button {
  font-family: sans-serif;
  background: #03a9f4;
  font-size: 11px;
  border: none;
  border-radius: 5px;
  color: #fff;
  cursor: pointer;
  font-weight: 600;
  padding: 10px 20px;
  letter-spacing: 2px;
  outline: none;
  text-transform: uppercase;
  text-decoration: none;
  margin: 2px 10px 2px 0;
  display: inline-block;
}
.box input[type="submit"]:hover,
.box button[type="submit"]:hover,
a.button:hover {
  opacity: 0.8;
}
.invalid-feedback {
  color: red;
  margin-bottom: 5px;
}
.response {
  color: white;
  margin-top: 25px;
}
table {
    color: white;
    width: 100%;
    border-collapse: collapse;
    margin-top: 20px;
  }
  
  table td {
    color: white;
  }
  table th {
    color: white;
  }
  thead tr {
  background-color: #222;
  color: white;
}

thead th {
  padding: 10px;
  text-align: center;
}

tbody tr:nth-child(even) {
  background-color: #555;
}

tbody td {
  padding: 10px;
  text-align: center;
}
tr.due-soon {
  background-color: red;
}
</style>