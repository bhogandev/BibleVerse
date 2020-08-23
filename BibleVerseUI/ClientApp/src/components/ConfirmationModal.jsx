import React, { useState } from 'react';
import { Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';

const ConfirmationModal = props => {
    //const [showModal, openModal] = useState(props.isOpen);

    function openModal(prop) {
        prop = !prop;
    }

    return (
        <Modal isOpen={props.isOpen} toggle={() => openModal(!props.isOpen)}>
            <ModalBody>
                {props.Body}
             </ModalBody>
            <ModalFooter>
                <Button onClick={props.OnClickFirst}>Yes</Button>
                <Button onClick={props.OnClickSecond}>No</Button>
            </ModalFooter>
        </Modal>
        )

}

export default ConfirmationModal