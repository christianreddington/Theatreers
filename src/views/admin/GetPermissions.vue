<template name="GetUsers">
  <div class="permissions">  
    <b-breadcrumb :items="breadcrumbs" id="breadcrumbs" v-if="breadcrumbs"></b-breadcrumb>
     <h1>Permissions</h1>
      <input v-model="search" class="form-control" placeholder="Filter users by Display Name"><br><br>
        <table class="table table-hover" id="table">
          <thead>
            <tr>
              <th v-for="column in columns">
                <a href="#" v-on:click="sort(column.itemsKey)">
                  {{ column.displayName }}
                </a>
              </th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in sortedItems">
              <td>{{ item.objectId }}</td>
              <td>{{ item.displayName }}</td>
              <td v-bind:id="item.objectId" v-bind:ref="item.objectId" contenteditable @focus="enterCell(item.objectId)" @blur="exitCell(item.objectId)">{{ item.extension_309951ebe380415e84418cf29a596f64_permissions }}</td>
            </tr>
          </tbody>
        </table>
</div>
</template>

<script>

export default {
    data() {
      return {
        items: [],
        dirtyPermission: null,
        columns: [
          {
            'itemsKey': 'objectId',
            'displayName': 'Object ID'
          },
          {
            'itemsKey': 'displayName',
            'displayName': 'Display Name'
          },
          {
            'itemsKey': 'extension_309951ebe380415e84418cf29a596f64_permissions',
            'displayName': 'Permissions'
          }
          ],
        cleanPermission: null,
        currentSort:'displayName',
        currentSortDir:'asc',
        search: '',

        visible: true,
        breadcrumbs: [],
        alert: {
          visible: false,
          type: 'info',
          content: 'No message to display'
        }
      }
    },
    computed: {
      filteredItems: function() {
        return this.items.filter(item => {
          return item.displayName.toLowerCase().includes(this.search.toLowerCase())
        })
      },      
    sortedItems:function() {
      return this.filteredItems.sort((a,b) => {
        let modifier = 1;
        if(this.currentSortDir === 'desc') modifier = -1;
        if(a[this.currentSort] < b[this.currentSort]) return -1 * modifier;
        if(a[this.currentSort] > b[this.currentSort]) return 1 * modifier;
        return 0;
      });
    }
    },
    methods: {    
  sort:function(s) {
    //if s == current sort, reverse
    if(s === this.currentSort) {
      this.currentSortDir = this.currentSortDir==='asc'?'desc':'asc';
    }
    this.currentSort = s;
  },
      enterCell: function (cellIdentifier){
        this.dirtyPermission = this.$refs[cellIdentifier][0].innerText;
      },
      exitCell: function (cellIdentifier){
        this.cleanPermission = this.$refs[cellIdentifier][0].innerText;
        console.log("Exiting Cell " + cellIdentifier)

        var id=this.$refs[cellIdentifier][0].id
        var j={"PermissionString":this.cleanPermission};
        var jsonBody = JSON.stringify(j); // '{"name":"binchen"}

        var tokenRequest = {
        scopes: ['https://theatreers.onmicrosoft.com/permissions/user_impersonation']
      };

        getAccessToken(tokenRequest)
        .then(bearerToken => { 
          putApiWithToken('https://th-admin-dev-weu-func.azurewebsites.net/api/moderation/permission/' + id, jsonBody, bearerToken.accessToken)  
          .catch(function (error) { 
            console.log("Error: " + error)
          })
          .then(function (response) { 
            console.log("Succeeded")
          })
        });
        this.cleanPermission = null
        this.dirtyPermission = null
      }
    },
    mounted: function () {       
      this.breadcrumbs = [
        {
          text: 'Theatreers',
          href: this.$router.resolve({ name: 'root' }).href
        },
        {
          text: 'Admin',
          href: this.$router.resolve({ name: 'AdminRoot' }).href
        },
        {
          text: 'Get Permissions',
          active: true
        }
      ];
      
      var tokenRequest = {
        scopes: ['https://theatreers.onmicrosoft.com/permissions/user_impersonation']
      };

        getAccessToken(tokenRequest)
        .then(bearerToken => { 
          getApiWithToken(`https://th-admin-dev-weu-func.azurewebsites.net/api/moderation/permission/`, bearerToken.accessToken)  
          .catch(function (error) { 
            self.alert = {
              visible: true,
              content: `${error}`,
              type: 'danger',
              display: true
            }
          })
          .then(function (response) {
            return response.json()
          })
          .then((jsonData) => {
            this.isLoading = false
            this.items = jsonData.value
          })
        });
    }
}
</script>
