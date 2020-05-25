import './App.css';
import clsx from 'clsx';
import { useHistory } from "react-router-dom";
import { makeStyles } from '@material-ui/core/styles';
import React, { useState } from 'react';
import { Switch, Route, Redirect } from 'react-router-dom';
import { Menu } from './components/Menu';
import { Login } from './pages/Login';
import { Dashboard } from "./pages/Dashboard";
import { Upload } from "./pages/Upload";
import DatasetDetails from './pages/DatasetDetails';
import { getToken } from "./actions/apiActions";

const drawerWidth = 240;

const useStyles = makeStyles(theme => ({
  content: {
    flexGrow: 1,
    transition: theme.transitions.create('margin', {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.leavingScreen,
    })
  },
  contentShift: {
    transition: theme.transitions.create('margin', {
      easing: theme.transitions.easing.easeOut,
      duration: theme.transitions.duration.enteringScreen,
    }),
    marginLeft: drawerWidth,
  },
}));

const App = () => {

  const history = useHistory();
  const classes = useStyles();
  const [isLoggedIn, setIsLoggedIn] = useState(getToken() !== null);

  const onLogout = () => {
    setIsLoggedIn(false);
    history.push("login");
  }

  const onLogin = () => {
    setIsLoggedIn(true);
    history.push("dashboard");
  }

  return (
    <React.Fragment>
      <Menu
        open={isLoggedIn}
        drawerWidth={drawerWidth}
        onLogout={onLogout}
      />

      <main
        className={clsx(classes.content, {
          [classes.contentShift]: isLoggedIn,
        })}
      >
        <Switch>
          <Route exact path='/'>
            <Redirect to="/login" />
          </Route>
          <Route path="/login">
            <Login setLogged={onLogin} />
          </Route>
          <Route path="/dashboard">
            <Dashboard />
          </Route>
          <Route path="/upload">
            <Upload />
          </Route>
          <Route path="/datasets/:id">
            <DatasetDetails />
          </Route>
        </Switch>
      </main>

    </React.Fragment>
  );
}

export default App;