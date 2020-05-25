import React, { useState } from "react";
import { makeStyles } from '@material-ui/core/styles';
import ListItem from '@material-ui/core/ListItem';
import ListItemText from '@material-ui/core/ListItemText';
import Divider from '@material-ui/core/Divider';
import Collapse from '@material-ui/core/Collapse';
import ExpandLess from '@material-ui/icons/ExpandLess';
import ExpandMore from '@material-ui/icons/ExpandMore';

const useStyles = makeStyles((theme) => ({
    divider: {
        marginRight: theme.spacing(2),
        marginLeft: theme.spacing(2)
    }
}));

export const OverflowedClusterTexts = ({ texts }) => {

    const classes = useStyles();

    const [show, setShow] = useState(false);

    if (!texts.length) return null;
    
    const textViews = texts.map((text, idx) => (
        <React.Fragment key={idx}>
            <ListItem>
                <ListItemText primary={text} />
            </ListItem>
            <Divider className={classes.divider} />
        </React.Fragment>
    ));

    const expanderButton = (
        <ListItem button onClick={() => setShow(!show)}>
            <ListItemText primary={show ? "Менше..." : "Більше..."} />
            {show ? <ExpandLess /> : <ExpandMore />}
        </ListItem>
    );

    return (
        <React.Fragment>
            {!show && expanderButton}
            <Collapse in={show} timeout="auto" unmountOnExit>
                {textViews}
            </Collapse>
            {show && expanderButton}
        </React.Fragment >
    );
};

export default OverflowedClusterTexts;