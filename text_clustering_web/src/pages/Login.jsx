import React, { useState, useEffect } from 'react';
import Button from '@material-ui/core/Button';
import CssBaseline from '@material-ui/core/CssBaseline';
import TextField from '@material-ui/core/TextField';
import Link from '@material-ui/core/Link';
import Paper from '@material-ui/core/Paper';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import { makeStyles } from '@material-ui/core/styles';
import { login, deleteToken } from '../actions/apiActions';

const useStyles = makeStyles((theme) => ({
    root: {
        height: '100vh',
    },
    image: {
        backgroundImage: 'url(https://source.unsplash.com/random)',
        backgroundRepeat: 'no-repeat',
        backgroundColor:
            theme.palette.type === 'light' ? theme.palette.grey[50] : theme.palette.grey[900],
        backgroundSize: 'cover',
        backgroundPosition: 'center',
    },
    paperContainer: {
        display: "flex",
        flexDirection: "column",
        justifyContent: "center"
    },
    paper: {
        margin: theme.spacing(8, 4),
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
    },
    form: {
        width: '100%', // Fix IE 11 issue.
        marginTop: theme.spacing(1),
    },
    submit: {
        margin: theme.spacing(3, 0, 2),
    },
}));

export const Login = ({ setLogged }) => {

    const classes = useStyles();
    const [credentials, setCredentials] = useState({
        email: "",
        password: ""
    });

    useEffect(deleteToken, []);

    const handleChangeForm = name => event => {
        setCredentials({ ...credentials, [name]: event.target.value });
    };

    const onLogin = event => {
        event.preventDefault();
        login(credentials).then(isLogged => isLogged && setLogged());
    }

    return (
        <Grid container component="main" className={classes.root}>
            <CssBaseline />
            <Grid item xs={false} sm={4} md={7} className={classes.image} />
            <Grid item xs={12} sm={8} md={5} component={Paper} className={classes.paperContainer} elevation={6} square>
                <div className={classes.paper}>
                    <Typography component="h3" variant="h5">
                        Введіть ваші дані
                    </Typography>
                    <form
                        className={classes.form}
                        onSubmit={onLogin}>
                        <TextField
                            variant="outlined"
                            margin="normal"
                            required
                            fullWidth
                            id="email"
                            label="Пошта"
                            name="email"
                            value={credentials.email}
                            onChange={handleChangeForm("email")}
                            autoComplete="email"
                            autoFocus
                        />
                        <TextField
                            variant="outlined"
                            margin="normal"
                            required
                            fullWidth
                            name="password"
                            value={credentials.password}
                            onChange={handleChangeForm("password")}
                            label="Пароль"
                            type="password"
                            id="password"
                            autoComplete="current-password"
                        />
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            color="primary"
                            className={classes.submit}
                        >
                            Увійти
                        </Button>
                        <Grid container>
                            <Grid item xs>
                            </Grid>
                            <Grid item>
                                <Link href="#" variant="body2">
                                    {"Не маєте акаунта? Зареєструйтесь"}
                                </Link>
                            </Grid>
                        </Grid>
                    </form>
                </div>
            </Grid>
        </Grid>
    );
}

export default Login;